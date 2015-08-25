using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Umbraco.Core;
using Umbraco.Core.Models;
using umbraco.editorControls.SettingControls.Pickers;

namespace Bytefunds.Cms.Logic.Helpers
{
    public class AutoPublishNewsHelper
    {
        public class RssNews
        {
            public string Title;
            // public string PubDate;
            public string Description;
        }

        public class RssReader
        {
            public static List<RssNews> GetNewsFromRss(string url, int num, out string catchNewsStatusInfo)
            {
                List<RssNews> rssNewsList = new List<RssNews>();
                catchNewsStatusInfo = string.Empty;
                try
                {
                    var webResponse = WebRequest.Create(url.Trim()).GetResponse();
                    if (webResponse == null)
                        return null;
                    var ds = new DataSet();
                    ds.ReadXml(webResponse.GetResponseStream());
                    var allNewsFromWeb = ds.Tables["item"].AsEnumerable().ToList();
                    int getNewsNum = allNewsFromWeb.Count() >= num ? num : allNewsFromWeb.Count();
                    for (int i = 0; i < getNewsNum; i++)
                    {
                        RssNews lastRssNew = new RssNews
                        {
                            Title = allNewsFromWeb[i].Field<string>("title"),
                            // PubDate = allNewsFromWeb[i].Field<string>("pubDate"),
                            Description = allNewsFromWeb[i].Field<string>("description")
                        };
                        //去除一些常见的不用的正文部分
                        if (lastRssNew.Description.IndexOf("发帖区") >= 0)
                        {
                            lastRssNew.Description = lastRssNew.Description.Remove(lastRssNew.Description.IndexOf("发帖区"));
                        }
                        if (lastRssNew.Description.IndexOf("责任编辑") >= 0)
                        {
                            lastRssNew.Description = lastRssNew.Description.Remove(lastRssNew.Description.IndexOf("责任编辑"));
                        }
                        if (lastRssNew.Description.IndexOf("This entry passed") >= 0)
                        {
                            lastRssNew.Description = lastRssNew.Description.Remove(lastRssNew.Description.IndexOf("This entry passed"));
                        }
                        if (lastRssNew.Description.IndexOf("【免责声明】") >= 0)
                        {
                            lastRssNew.Description = lastRssNew.Description.Remove(lastRssNew.Description.IndexOf("【免责声明】"));
                        }
                        rssNewsList.Add(lastRssNew);
                    }
                    catchNewsStatusInfo = DateTime.Now.ToString("yyyy-M-d dddd HH:mm:ss") + "：新闻抓取成功。"+"\r\n";
                    return rssNewsList;
                }
                catch (Exception ex)
                {
                    catchNewsStatusInfo = DateTime.Now.ToString("yyyy-M-d dddd HH:mm:ss") + "：新闻抓取失败。错误内容:" + ex+"\r\n";

                    return null;
                }

            }
        }


        public static bool PublishNew(string contentName, int parentContentId, RssNews news, out string publishInfo)
        {
            try
            {
                string contentTypeAlias = "InfoDetail";
                string contentAddress = contentName + DateTime.Now.ToString("yyyyMMddHHmmss");
                //RssNews publishNews = RssReader.ReadLastNew(rssAddress);
                IContent newContent = ApplicationContext.Current.Services.ContentService.CreateContent(contentAddress, parentContentId,
                    contentTypeAlias, 0);
                newContent.SetValue("bodyText", news.Description.Trim());
                newContent.SetValue("pageTitle", news.Title);
                ApplicationContext.Current.Services.ContentService.Publish(newContent, 0);
                publishInfo = DateTime.Now.ToString("yyyy-M-d dddd HH:mm:ss") +"：发布成功。发布的新闻标题：" + news.Title + "\r\n ---------------------------\r\n";
                return true;

            }
            catch (Exception ex)
            {
                //Log.Add(LogTypes.Publish,nodeID,"自动发布rss新闻时出错:"+ex.Message );
                publishInfo = DateTime.Now.ToString("yyyy-M-d dddd HH:mm:ss") + "：发布错误,请查看下方错误消息。\r\n发布新闻标题：" + news.Title + "\r\n错误消息：" + ex.Message + "\r\n ---------------------------\r\n";
                return true;
            }

        }

        public static void AutoGetAndPublish()
        {
            //查找所有有url的OverView类型的发布
            IContentType type = ApplicationContext.Current.Services.ContentTypeService.GetContentType("Overview");
            List<IContent> needPublishContents =
                ApplicationContext.Current.Services.ContentService.GetContentOfContentType(type.Id).Where(c => c.ParentId.Equals(1075))
                    .Where(c => c.GetValue("url").ToString().Trim() != string.Empty)
                    .ToList();
            //比较是否有新的新闻要发布
            foreach (IContent newsContent in needPublishContents)
            {
                if (newsContent.GetValue<bool>("StartGettingNews").Equals(true))
                {

                    IContent lastPublishNews =
                        newsContent.Children().OrderByDescending(c => c.CreateDate).FirstOrDefault();

                    string catchNewsInfo = string.Empty;

                    List<RssNews> rssNews = RssReader.GetNewsFromRss(newsContent.GetValue("url").ToString().Trim(),
                        1, out catchNewsInfo);
                    string oldPublishStatus = newsContent.GetValue("publishStatus").ToString().Trim();
                    if (rssNews != null && rssNews.Count() > 0)
                    {
                        if (rssNews[0].Title.Trim() !=
                                lastPublishNews.GetValue("pageTitle").ToString().Trim())
                        {
                            
                            string newPublishStatus = string.Empty;
                            bool isSuccess = PublishNew(newsContent.GetValue("AddressPrefix").ToString().Trim(),
                                newsContent.Id, rssNews[0], out newPublishStatus); //执行发布
                            newsContent.SetValue("publishStatus", oldPublishStatus+catchNewsInfo + newPublishStatus);
                            if (isSuccess == false)
                            {
                                //发布失败，设置为停止发布
                                // newsContent.SetValue("StartGettingNews", false);
                                ApplicationContext.Current.Services.ContentService.Publish(newsContent, 0);

                            }
                        }
                        else //一样的新闻不再发布
                        {
                            string newPublishStatus = DateTime.Now.ToString("yyyy-M-d dddd HH:mm:ss") +
                                                      "：已执行抓取新闻，但没有新的新闻。\r\n---------------------------\r\n";
                            newsContent.SetValue("publishStatus", oldPublishStatus + newPublishStatus);
                            ApplicationContext.Current.Services.ContentService.Publish(newsContent, 0);
                            break;
                        }

                    }
                    else//抓取新闻失败
                    {
                        //newsContent.SetValue("StartGettingNews", false);
                        newsContent.SetValue("publishStatus", catchNewsInfo);
                        ApplicationContext.Current.Services.ContentService.Publish(newsContent, 0);
                    }

                }
            }



        }
    }
}