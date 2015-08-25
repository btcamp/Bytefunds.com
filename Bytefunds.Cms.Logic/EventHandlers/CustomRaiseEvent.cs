using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Bytefunds.Cms.Logic.EventHandlers
{
    public class CustomRaiseEvent
    {
        public static void RaiseRegistered(IMember m)
        {
            Registered.RaiseEvent(new NewEventArgs<IMember>(m, false, m.ContentTypeAlias, -1), null); 
        }
        public static void RaiseContentCreated(IContent content)
        {
            ContentCreated.RaiseEvent(new NewEventArgs<IContent>(content, false, content.ContentType.Alias, content.ParentId), null);
        }
        public static event TypedEventHandler<IContentService, NewEventArgs<IContent>> ContentCreated;
        public static event TypedEventHandler<IMemberService, NewEventArgs<IMember>> Registered;
    }
}