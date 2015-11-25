angular.module("umbraco").controller("UmbracoForms.RenderTypes.FileController",
    function($scope){

       

        var imageExts = ['jpg','jpeg','png','gif','bmp'];

        $scope.files = $scope.field.replace('~', '').split(',');

        $scope.isImage = function(filepath){
            return imageExts.indexOf( $scope.getExtension(filepath) ) >= 0;
        };

        $scope.getExtension = function(filepath){
            return filepath.substring(filepath.lastIndexOf(".")+1).toLowerCase();
        };

        $scope.getFileName = function(filepath){
            return filepath.substring(filepath.lastIndexOf("/")+1);
        };

        $scope.getPreview = function(filepath){
            return filepath.replace('~','') + "?width=400";
        };

    });

angular.module("umbraco").controller("UmbracoForms.SettingTypes.DocumentMapperController",
	function ($scope, $routeParams,pickerResource) {

	    if (!$scope.setting.value) {
	       
	    } else {
	        var value = JSON.parse($scope.setting.value);
	        $scope.doctype = value.doctype;
	        $scope.nameField = value.nameField;
	        $scope.nameStaticValue = value.nameStaticValue;
	        $scope.properties = value.properties;
	    }

	    pickerResource.getAllDocumentTypesWithAlias().then(function (response) {
	        $scope.doctypes = response.data;
	    });

	    pickerResource.getAllFields($routeParams.id).then(function (response) {
	        $scope.fields = response.data;
	    });

	    $scope.setDocType = function() {

	        pickerResource.getAllProperties($scope.doctype).then(function (response) {
	            $scope.properties = response.data;
	        });
	    };

	    $scope.setValue = function() {
	       
	        var val = {};
	        val.doctype = $scope.doctype;
	        val.nameField = $scope.nameField;
	        val.nameStaticValue = $scope.nameStaticValue;
	        val.properties = $scope.properties;

	        $scope.setting.value = JSON.stringify(val);
	    };
	});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.FieldMapperController",
	function ($scope, $routeParams, pickerResource) {

	    if (!$scope.setting.value) {
	        $scope.mappings = [];
	    } else {
	        $scope.mappings = JSON.parse($scope.setting.value);
	    }

        pickerResource.getAllFields($routeParams.id).then(function (response) {
            $scope.fields = response.data;
        });

        $scope.addMapping = function () {
            $scope.mappings.push({
                alias: $scope.alias,
                value: $scope.value,
                staticValue: $scope.staticValue
        });
	        $scope.alias = '';
	        $scope.value = '';
	        $scope.staticValue = '';
	        $scope.setting.value = JSON.stringify($scope.mappings);
        };

	    $scope.deleteMapping = function(index) {
	        $scope.mappings.splice(index, 1);
	        $scope.setting.value = JSON.stringify($scope.mappings);
	    };
	});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.File",
	function ($scope, dialogService) {

	    $scope.openMediaPicker = function() {
	        dialogService.mediaPicker({ callback: populateFile });
	    };

        function populateFile(item) {
            $scope.setting.value = item.properties[0].value;
        }
	});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.ConnectionStringController",
	function ($scope, $routeParams, pickerResource) {
	    pickerResource.getAllConnectionStrings().then(function (response) {
	        $scope.strings = response.data;
	    });
	});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.ContentController",
	function ($scope, $routeParams, dialogService, entityResource, iconHelper) {

	if (!$scope.setting) {
	    $scope.setting = {};
	}


	var val = parseInt($scope.setting.value);
	

	if (!isNaN(val) && angular.isNumber(val)) {
	    //node
	    $scope.showQuery = false;

	    entityResource.getById($scope.setting.value, "Document").then(function (item) {
	        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	        $scope.node = item;
	    });
	} 

	$scope.openContentPicker = function () {
	    var d = dialogService.treePicker({
	        section: "content",
	        treeAlias: "content",
	        multiPicker: false,
	        callback: populate
	    });
	};


	$scope.clear = function () {
	    $scope.id = undefined;
	    $scope.node = undefined;
	    $scope.setting.value = undefined;
	};

	function populate(item) {
	    $scope.clear();
	    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	    $scope.node = item;
	    $scope.id = item.id;
	    $scope.setting.value = item.id;
	}

});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.ContentWithXpathController",
	function ($scope, $routeParams, dialogService, entityResource, iconHelper) {

	if (!$scope.setting) {
	    $scope.setting = {};
	}

	var val = parseInt($scope.setting.value);
	

	if (!isNaN(val) && angular.isNumber(val)) {
	    //node
	    $scope.showQuery = false;

	    entityResource.getById($scope.setting.value, "Document").then(function (item) {
	        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	        $scope.node = item;
	    });
	} else {
	    //xpath
	    $scope.showQuery = true;
	    $scope.query = $scope.setting.value;
	}

	$scope.openContentPicker = function () {
	    var d = dialogService.treePicker({
	        section: "content",
	        treeAlias: "content",
	        multiPicker: false,
	        callback: populate
	    });
	};

	$scope.setXpath = function() {
	    $scope.setting.value = $scope.query;
	};

	$scope.clear = function () {
	    $scope.id = undefined;
	    $scope.node = undefined;
	    $scope.setting.value = undefined;
	};

	function populate(item) {
	    $scope.clear();
	    item.icon = iconHelper.convertFromLegacyIcon(item.icon);
	    $scope.node = item;
	    $scope.id = item.id;
	    $scope.setting.value = item.id;
	}

});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.DataTypeController",
	function ($scope, $routeParams, pickerResource) {
	    pickerResource.getAllDataTypes().then(function (response) {
	        $scope.datatypes = response.data;
	    });
	});
angular.module("umbraco").controller("UmbracoForms.SettingTypes.Pickers.DocumentTypeController",
	function ($scope, $routeParams, pickerResource) {
	    pickerResource.getAllDocumentTypesWithAlias().then(function (response) {
	        $scope.doctypes = response.data;
	    });
	});
angular.module("umbraco")
.controller("UmbracoForms.Dashboards.ActivityController",
	function ($scope, recordResource) {

		//var filter = {};
		//recordResource.getRecords(filter).then(function(response){
		//	$scope.records = response.data;
		//});

	});

angular.module("umbraco")
.controller("UmbracoForms.Dashboards.LicensingController",
    function ($scope, $location, $routeParams, $cookieStore, formResource, licensingResource, updatesResource, notificationsService) {
        
        $scope.overlay = {
            show: false,
            title: "Congratulations",
            description: "You've just installed Umbraco Forms - Let's create your first form"
        };

        var packageInstall = $cookieStore.get("umbPackageInstallId");
        if(packageInstall){
            $scope.overlay.show = true; 
            $cookieStore.put("umbPackageInstallId", ""); 
        }

        //if not initial install, but still do not have forms
        if(!$scope.overlay.show){
            formResource.getOverView().then(function(response){
                if(response.data.length === 0){
                    $scope.overlay.show = true;
                    $scope.overlay.title = "Create a form";
                    $scope.overlay.description = "You do not have any forms setup yet, how about creating one now?";
                }    
            });
        }

        $scope.getLicenses = function(config){

            $scope.loginError = false;
            $scope.hasLicenses = undefined;

            licensingResource.getAvailableLicenses(config).then(function(response){
                var licenses = response.data;
                var currentDomain = window.location.hostname;

                $scope.hasLicenses = licenses.length > 0;
                _.each(licenses, function(lic){
                    if(lic.bindings && lic.bindings.indexOf(currentDomain) >= 0){
                        lic.currentDomainMatch = true;
                    }
                });

                $scope.configuredLicenses = _.filter(licenses, function(license){ return license.configured; });
                $scope.openLicenses = _.filter(licenses, function(license){ return license.configured === false; });

            }, function(err){
                $scope.loginError = true;
                $scope.hasLicenses = undefined;
            });
        };


        $scope.configure = function(config){
            licensingResource.configureLicense(config).then(function(response){
                $scope.configuredLicenses.length = 0;
                $scope.openLicenses.length = 0;
                $scope.loadStatus();

                notificationsService.success("License configured", "Umbraco forms have been configured to be used on this website");
            });
        };

        $scope.loadStatus = function(){
            licensingResource.getLicenseStatus().then(function(response){
                    $scope.status = response.data;
            });

            updatesResource.getUpdateStatus().then(function(response){
                $scope.version = response.data;
            });
        };

        $scope.upgrade = function(){
            $scope.installing = true;
            updatesResource.installLatest($scope.version.remoteVersion).then(function(response){
                window.location.reload();
            });
        };


        $scope.create= function(){
            $location.url("forms/form/edit/-1?template=&create=true");
        };


        $scope.configuration = {domain: window.location.hostname};
        $scope.loadStatus();
    });

angular.module("umbraco")
.controller("UmbracoForms.Dashboards.YourFormsController",
    function ($scope, formResource, recordResource) {

            $scope.formsLimit = 4;
            $scope.showAll = function(){
                $scope.formsLimit = 50;
            };

            formResource.getOverView().then(function(response){
                    $scope.forms = response.data;

                    _.each($scope.forms, function(form){
                            var filter = {form: form.id};
                            recordResource.getRecordsCount(filter).then(function(response){
                                    form.entries = response.data.count;
                            });
                    });
            });
    });

angular.module("umbraco")
.controller("UmbracoForms.Editors.DataSource.DeleteController",
	function ($scope, dataSourceResource, navigationService, treeService) {
	    $scope.delete = function (id) {
	        dataSourceResource.deleteByGuid(id).then(function () {

	            treeService.removeNode($scope.currentNode);
	            navigationService.hideNavigation();

	        });

	    };
	    $scope.cancelDelete = function () {
	        navigationService.hideNavigation();
	    };
	});
angular.module("umbraco").controller("UmbracoForms.Editors.DataSource.EditController",
	function ($scope, $routeParams, dataSourceResource, editorState, notificationsService, dialogService, navigationService) {
	    if ($routeParams.create) {
	        //we are creating so get an empty data type item
	        dataSourceResource.getScaffold()
			.then(function (response) {
			    $scope.loaded = true;
			    $scope.dataSource = response.data;

			    dataSourceResource.getAllDataSourceTypesWithSettings()
		        .then(function (resp) {
		            $scope.types = resp.data;

		        });

			    //set a shared state
			    editorState.set($scope.form);
			});
	    } else {
	        //we are editing so get the content item from the server
	        dataSourceResource.getByGuid($routeParams.id)
            .then(function (response) {

                $scope.dataSource = response.data;

                dataSourceResource.getAllDataSourceTypesWithSettings()
                    .then(function (resp) {
                        $scope.types = resp.data;
                        setTypeAndSettings();
    
                        $scope.loaded = true;
                    });



                //set a shared state
                editorState.set($scope.dataSource);
            });
	    }

	    $scope.setType = function () {
	        setTypeAndSettings();
	    };

	    $scope.save = function () {
	
	        //set settings
	        $scope.dataSource.settings = {};
	        angular.forEach($scope.dataSource.$type.settings, function (setting) {
	            var key = setting.alias;
	            var value = setting.value;
	            $scope.dataSource.settings[key] = value;
	           
	        });
	        //validate settings
	        dataSourceResource.validateSettings($scope.dataSource)
            .then(function (response) {

                $scope.errors = response.data;

                if ($scope.errors.length > 0) {
                    $scope.dataSource.valid = false;
                    angular.forEach($scope.errors, function (error) {

                        notificationsService.error("Datasource failed to save", error.Message);
                       
                    });
                } else {
                    //save
                    dataSourceResource.save($scope.dataSource)
                    .then(function (response) {

                        $scope.dataSource = response.data;
                        //set a shared state
                        editorState.set($scope.dataSource);
                        setTypeAndSettings();
                        navigationService.syncTree({ tree: "datasource", path: [String($scope.dataSource.id)], forceReload: true });
                        notificationsService.success("Datasource saved", "");
                        $scope.dataSource.valid = true;
                        $scope.dataSourceForm.$dirty = false;
                    }, function (err) {
                        notificationsService.error("Datasource failed to save", "");
                    });
                }


            }, function (err) {
                notificationsService.error("Datasource failed to save", "Please check if your settings are valid");
            });

	    };

	    $scope.showWizard = function() {
	        dialogService.open({
	            template: "/app_plugins/UmbracoForms/Backoffice/Datasource/dialogs/wizard.html",
	            dataSourceId: $scope.dataSource.id
            });
	    };

	    var setTypeAndSettings = function () {
	        $scope.dataSource.$type = _.where($scope.types, { id: $scope.dataSource.formDataSourceTypeId })[0];

	        //set settings
	        angular.forEach($scope.dataSource.settings, function (setting) {
	            for (var key in $scope.dataSource.settings) {
	                if ($scope.dataSource.settings.hasOwnProperty(key)) {
	                    if (_.where($scope.dataSource.$type.settings, { alias: key }).length > 0) {
	                        _.where($scope.dataSource.$type.settings, { alias: key })[0].value = $scope.dataSource.settings[key];
	                    }

	                }
	            }
	        });
	    };



	});
angular.module("umbraco")
.controller("UmbracoForms.Editors.DataSource.WizardController",
	function ($scope, $routeParams, dataSourceWizardResource, navigationService, notificationsService, dialogService) {

	    $scope.currentStep = 1;

	    dataSourceWizardResource.getScaffold($scope.dialogOptions.dataSourceId).then(function (response) {

	         $scope.wizard = response.data;

	         $scope.hasPrimaryKeys = $scope.wizard.mappings.length != _.where($scope.wizard.mappings, { prevalueKeyField: null }).length;

	         dataSourceWizardResource.getAllFieldTypes()
                 .then(function (resp) {
                     $scope.fieldtypes = resp.data;
                     $scope.ready = true;
                 });
	     });


	    $scope.createForm = function() {

	        dataSourceWizardResource.createForm($scope.wizard)
	            .then(function (resp) {
	                dialogService.closeAll();
	                notificationsService.success("Form created", "");
	            });
	    };

	    $scope.gotoStep = function (step) {
	        $scope.currentStep = step;
	    }

	    $scope.gotoThirdStep = function() {
	        if ($scope.hasPrimaryKeys) {
	            $scope.currentStep = 3;
	        } else {
	            $scope.currentStep = 4;
	        }
	    }
        $scope.goBackToThirdStep = function() {
            if ($scope.hasPrimaryKeys) {
                $scope.currentStep = 3;
            } else {
                $scope.currentStep = 2;
            }
        }
	});
angular.module("umbraco").controller("UmbracoForms.Editors.Form.CreateController",
	function ($scope, $routeParams, formResource, editorState, notificationsService) {
		formResource.getAllTemplates().then(function(response) {
		    $scope.formTemplates = response.data;
		});
});
angular.module("umbraco")
.controller("UmbracoForms.Editors.Form.DeleteController",
	function ($scope, formResource, navigationService, treeService) {
	    $scope.delete = function (id) {
	        formResource.deleteByGuid(id).then(function () {

	            treeService.removeNode($scope.currentNode);
	            navigationService.hideNavigation();

	        });

	    };
	    $scope.cancelDelete = function () {
	        navigationService.hideNavigation();
	    };
	});
angular.module("umbraco").controller("UmbracoForms.Editors.Form.EditController",

function ($scope, $routeParams, formResource, editorState, dialogService, formService, notificationsService, contentEditingHelper, formHelper, navigationService) {
	if ($routeParams.create) {

		//we are creating so get an empty data type item
	    formResource.getScaffold($routeParams.template)
	        .then(function(response) {
	            $scope.form = response.data;
				$scope.currentPage = {};

	            formResource.getPrevalueSources()
	                .then(function(resp){
	                    $scope.prevaluesources = resp.data;
	            });

				formResource.getAllFieldTypesWithSettings()
					.then(function (resp) {
						$scope.fieldtypes = resp.data;
						$scope.ready = true;
					});

				//set a shared state
				editorState.set($scope.form);
			});

	} else {

		$scope.workflowsUrl = "#/forms/form/workflows/" +$routeParams.id;
		$scope.entriesUrl = "#/forms/form/entries/" +$routeParams.id;

		//we are editing so get the content item from the server
		formResource.getByGuid($routeParams.id)
			.then(function(response){
				$scope.form = response.data;
				$scope.saved = true;

				formResource.getPrevalueSources()
	                .then(function (resp) {
	                    $scope.prevaluesources = resp.data;
	                });

				formResource.getAllFieldTypesWithSettings()
					.then(function (resp) {
						$scope.fieldtypes = resp.data;
						$scope.ready = true;
					});

				//set a shared state
				editorState.set($scope.form);
			});
	}

	$scope.editForm = function(form, section){
		dialogService.open(
			{
				template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/formsettings.html",
				form: form,
				section: section,
				page: $scope.currentPage
			});
	};

	$scope.save = function(){

		//make sure we set correct widths on all containers
		formService.syncContainerWidths($scope.form);

		formResource.save($scope.form)
			.then(function(response){

				formHelper.resetForm({ scope: $scope });

				contentEditingHelper.handleSuccessfulSave({
					scope: $scope,
					savedContent: response.data
				});

				$scope.ready = true;
				//$scope.form = response.data;

				//set a shared state
				editorState.set($scope.form);

				navigationService.syncTree({ tree: "form", path: [String($scope.form.id)], forceReload: true });

				notificationsService.success("Form saved", "");

			},function(err){
				notificationsService.error("Form Failed to save", err);
			});
	};

});

angular.module("umbraco").controller("UmbracoForms.Editors.Form.EntriesController",
	function($scope, $routeParams, recordResource, formResource, dialogService, editorState){

	formResource.getByGuid($routeParams.id)
		.then(function(response){
			$scope.form = response.data;
			$scope.loaded = true;
		});

	$scope.states = ["Approved", "Submitted"];

	$scope.filter = {
		startIndex: 1,
		length: 20,
		form: $routeParams.id,
		sortBy: "created",
		sortOrder: "descending",
		states: ["Approved","Submitted"]
	};

	$scope.records = [];
	recordResource.getRecordSetActions().then(function(response){
		$scope.recordSetActions = response.data;
	});

	recordResource.getRecordSetActions().then(function(response){
		$scope.recordActions = response.data;
	});

	$scope.toggleRecordStateSelection = function(recordState) {
	    var idx = $scope.filter.states.indexOf(recordState);

	    // is currently selected
	    if (idx > -1) {
	        $scope.filter.states.splice(idx, 1);
	    }

	        // is newly selected
	    else {
	        $scope.filter.states.push(recordState);
	    }
	};

	$scope.hiddenFields = [2];
	$scope.toggleSelection = function toggleSelection(field) {
	    var idx = $scope.hiddenFields.indexOf(field);

	    // is currently selected
	    if (idx > -1) {
	      $scope.hiddenFields.splice(idx, 1);
	    }else {
	      $scope.hiddenFields.push(field);
	    }
	};


	$scope.edit = function(schema){
	    dialogService.open(
	            {
	                template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/entriessettings.html",
	                schema: schema,
	                toggle: $scope.toggleSelection,
	                hiddenFields: $scope.hiddenFields,
					filter: $scope.filter
	            });
	};

	$scope.viewdetail = function(schema, row){
		dialogService.open(
				{
					template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/entriesdetail.html",
					schema: schema,
					row: row,
					hiddenFields: $scope.hiddenFields
				});
	};

	$scope.pagination = [];

	$scope.next = function(){
		$scope.filter.startIndex++;
	};

	$scope.prev = function(){
		$scope.filter.startIndex--;
	};

	$scope.gotoPage = function(index){
		$scope.filter.startIndex = index;
	};


	$scope.search = _.debounce(function(resetIndex){

		$scope.reset(resetIndex);

		$scope.$apply(function(){
			recordResource.getRecords($scope.filter).then(function(response){
				$scope.records = response.data;
				$scope.pagination.length = $scope.records.totalNumberOfPages;
			});
		});


	}, 300);


	$scope.$watch("filter", function(newVal, oldVal) {
	
		var resetIndex = newVal.filter !== oldVal.filter;
		$scope.search(resetIndex);

	}, true);


	$scope.loadRecords = function(filter, append){
		recordResource.getRecords(filter).then(function(response){
			if(append){
				$scope.records = $scope.records.results.concat(response.data.results);
			}else{
				$scope.records = response.data;
			}

			$scope.allIsChecked =  ($scope.selectedRows.length >= $scope.records.results.length);
		});
	};

	$scope.reset = function(resetIndex){
		$scope.selectedRows.length = 0;
		$scope.allIsChecked = false;

		if(resetIndex){
			$scope.filter.startIndex = 1;
		}

	};

	$scope.more = function(){
		$scope.filter.startIndex++;
		$scope.loadRecords($scope.filter, true);
	};

	$scope.selectedRows = [];
	$scope.toggleRow = function(row){
		if(row.selected){
			$scope.selectedRows.push(row.id);
			$scope.allIsChecked =  ($scope.selectedRows.length >= $scope.records.results.length);
		}else{
			var i = $scope.selectedRows.indexOf(row.id);
			$scope.selectedRows.splice(i,1);
			$scope.allIsChecked = false;
		}
	};

	$scope.allIsChecked = false;
	$scope.toggleAll = function($event){
		var checkbox = $event.target;
		$scope.selectedRows.length = 0;

		for (var i = 0; i < $scope.records.results.length; i++) {
			var entity = $scope.records.results[i];
			entity.selected = checkbox.checked;

			if(checkbox.checked){
				$scope.selectedRows.push(entity.id);
			}
		}
	};

	$scope.executeRecordSetAction = function(action){
		var model = {formId: $scope.form.id, recordKeys:$scope.selectedRows, actionId: action.id};
		recordResource.executeRecordSetAction(model).then(function(response){
			$scope.reset(true);
			$scope.loadRecords($scope.filter, false);
		});
	};
});

angular.module("umbraco").controller("UmbracoForms.Editors.Form.EntriesSettingsController",
    function($scope, $log, $timeout, exportResource){

        exportResource.getExportTypes().then(function(response){
            $scope.exportTypes = response.data;
        });

        $scope.export = function(type, filter){
            filter.exportType = type.id;
            exportResource.generateExport(filter).then(function(response){

                var url = exportResource.getExportUrl(response.data.file);
                var iframe = document.createElement('iframe');
                iframe.id = "hiddenDownloadframe";
                iframe.style.display = 'none';
                document.body.appendChild(iframe);
                iframe.src= url;

                //remove all traces
                $timeout(function(){
                    document.body.removeChild(iframe);
                }, 1000);
            });
        };

    });

angular.module("umbraco").controller("UmbracoForms.Editors.Form.Dialogs.FieldsetSettingController",
	function ($scope, formService, dialogService) {

	    $scope.deleteConditionRule = function(rules, rule) {
	        formService.deleteConditionRule(rules, rule);
	    };

	    $scope.addConditionRule = function (condition) {
	        formService.addConditionRule(condition);
	    };

        $scope.close = function() {
            dialogService.closeAll();
        }
	}
);

angular.module("umbraco").controller("UmbracoForms.Editors.Form.Dialogs.FieldSettingController",
	function ($scope, formService, dialogService) {

	    $scope.deleteConditionRule = function(rules, rule) {
	        formService.deleteConditionRule(rules, rule);
	    };

	    $scope.addConditionRule = function (condition) {
	        formService.addConditionRule(condition);
	    };

	    $scope.getPrevalues = function (field) {
	        
	        formService.loadFieldTypePrevalues(field);

	    };

        $scope.close = function() {
            
            $scope.dialogOptions.field.settings = {};
            angular.forEach($scope.dialogOptions.field.$fieldType.settings, function (setting) {
                var key = setting.alias;
                var value = setting.value;
                $scope.dialogOptions.field.settings[key] = value;
                dialogService.closeAll();
            });
        }
	});

angular.module("umbraco").controller("UmbracoForms.Editors.Form.WorkflowsController",
    function ($scope, $routeParams, workflowResource, editorState, dialogService, $window) {


        workflowResource.getAllWorkflows($routeParams.id)
            .then(function(resp) {
                $scope.workflows = resp.data;
                $scope.loaded = true;
            });

        $scope.sortableOptions = {
            handle: '.handle',
            cursor: "move",
            connectWith: '.workflows',
            update: function (e, ui) {
                var wfGuids = [];
                var wfcount = 0;
                var state = ui.item.parent().attr("rel");
                ui.item.parent().children().each(function () {
                    if ($(this).attr("rel") != null) {
                        wfGuids[wfcount] = $(this).attr("rel");
                        wfcount++;
                    }
                });

                workflowResource.updateSortOrder(state,wfGuids).then(function () {


                });

            },
        };

        $scope.deleteWorkflow = function (workflow) {
            var deleteWorkflow = $window.confirm('Are you sure you want to delete the workflow?');

            if (deleteWorkflow) {
                workflowResource.deleteByGuid(workflow.id).then(function() {
                    $scope.workflows.splice($scope.workflows.indexOf(workflow), 1);

                });
            }
        };

        $scope.updateWorkflow = function(state, workflow) {
            data = {};
            data.workflow = workflow;
            data.state = state;
            data.form = $routeParams.id;
            data.add = false;

            dialogService.open({
                template: '/app_plugins/UmbracoForms/Backoffice/Form/dialogs/workflow.html',
                show: true,
                callback: update,
                dialogData: data,
                workflows: $scope.workflows
        });
        };

        $scope.addWorkflow = function(state) {
            data = {};
            data.state = state;
            data.form = $routeParams.id;
            data.add = true;

            dialogService.open({
                template: '/app_plugins/UmbracoForms/Backoffice/Form/dialogs/workflow.html',
                show: true,
                callback: add,
                dialogData: data
            });
        };

        function add(data) {

            $scope.workflows.push(data);
        }

        function update(data) {
            
        }
    });
angular.module("umbraco").controller("UmbracoForms.Editors.Form.Dialogs.WorkflowsController",
	function ($scope, $routeParams, workflowResource, dialogService, notificationsService, $window) {

	    if ($scope.dialogData.workflow) {
	        //edit
	        $scope.workflow = $scope.dialogData.workflow;
	        workflowResource.getAllWorkflowTypesWithSettings()
	            .then(function (resp) {
	                $scope.types = resp.data;
	                setTypeAndSettings();
	            });

	        //workflowResource.getByGuid($scope.dialogData.workflow)
            //.then(function (response) {

            //    $scope.workflow = response.data;

            //    workflowResource.getAllWorkflowTypesWithSettings()
            //        .then(function (resp) {
            //            $scope.types = resp.data;
            //            setTypeAndSettings();
            //        });
                
            //});

	    } else {
	        //create
	        workflowResource.getScaffold()
	            .then(function(response) {
	                $scope.loaded = true;
	                $scope.workflow = response.data;
	                $scope.workflow.executesOn = $scope.dialogData.state;
	                $scope.workflow.form = $scope.dialogData.form;
	                $scope.workflow.active = true;

	                workflowResource.getAllWorkflowTypesWithSettings()
	                    .then(function(resp) {
	                        $scope.types = resp.data;

	                    });

	            });
	    }


	    $scope.setType = function () {
	        setTypeAndSettings();
	    };

	    $scope.close = function () {
	       
	        dialogService.closeAll();
	    };

	    $scope.add = function () {
	       
	        save();
	        
	    };

	    $scope.update = function () {
	       
	        save();
	        
	    };

        $scope.delete = function() {
            var deleteWorkflow = $window.confirm('Are you sure you want to delete the workflow?');

            if (deleteWorkflow) {
                workflowResource.deleteByGuid($scope.workflow.id).then(function () {
                    $scope.dialogOptions.workflows.splice($scope.dialogOptions.workflows.indexOf($scope.workflow), 1);

                    notificationsService.success("Workflow deleted", "");
                    //$scope.submit($scope.workflow);
                    dialogService.closeAll();

                });
            }
        }

	    var save = function() {
	        console.log('save');
	        //set settings
	        $scope.workflow.settings = {};
	        angular.forEach($scope.workflow.$type.settings, function (setting) {
	            var key = setting.alias;
	            var value = setting.value;
	            $scope.workflow.settings[key] = value;
	            console.log("set setting " + key + ":" + value);
	        });
	        //validate settings
	        workflowResource.validateSettings($scope.workflow)
                .then(function (response) {

                    $scope.errors = response.data;

                    if ($scope.errors.length > 0) {
                        angular.forEach($scope.errors, function (error) {

                            notificationsService.error("Workflow failed to save", error.Message);
                        });
                    } else {
                        //save
                        workflowResource.save($scope.workflow)
                        .then(function (response) {

                            $scope.workflow = response.data;
                           
                            setTypeAndSettings();
                           
                            notificationsService.success("Workflow saved", "");
                            $scope.submit($scope.workflow);
                            dialogService.closeAll();

                        }, function (err) {
                            notificationsService.error("Workflow failed to save", "");
                        });
                    }

                }, function (err) {
                    notificationsService.error("Workflow failed to save", "Please check if your settings are valid");
                });
	    };

	    var setTypeAndSettings = function () {
	        $scope.workflow.$type = _.where($scope.types, { id: $scope.workflow.workflowTypeId })[0];
	        if (!$scope.workflow.name) {
	            $scope.workflow.name = $scope.workflow.$type.name;
	        }
	        //set settings
	        angular.forEach($scope.workflow.settings, function (setting) {
	            for (var key in $scope.workflow.settings) {
	                if ($scope.workflow.settings.hasOwnProperty(key)) {
	                    if (_.where($scope.workflow.$type.settings, { alias: key }).length > 0) {
	                        _.where($scope.workflow.$type.settings, { alias: key })[0].value = $scope.workflow.settings[key];
	                    }

	                }
	            }
	        });
	    };
	});
angular.module("umbraco")
.controller("UmbracoForms.Editors.PreValueSource.DeleteController",
	function ($scope, preValueSourceResource, navigationService, treeService) {
	    $scope.delete = function (id) {
	        preValueSourceResource.deleteByGuid(id).then(function () {
	          
	            treeService.removeNode($scope.currentNode);
	            navigationService.hideNavigation();

	        });

	    };
	    $scope.cancelDelete = function () {
	        navigationService.hideNavigation();
	    };
	});
angular.module("umbraco").controller("UmbracoForms.Editors.PreValueSource.EditController",

function ($scope, $routeParams, preValueSourceResource, editorState, notificationsService, navigationService) {
    if ($routeParams.create) {
        //we are creating so get an empty data type item
        preValueSourceResource.getScaffold()
		.then(function (response) {
		    $scope.loaded = true;
		    $scope.preValueSource = response.data;

		    preValueSourceResource.getAllPreValueSourceTypesWithSettings()
	        .then(function (resp) {
	            $scope.types = resp.data;
	           
	        });

		    //set a shared state
		    editorState.set($scope.form);
		});
    } else {
        //we are editing so get the content item from the server
        preValueSourceResource.getByGuid($routeParams.id)
        .then(function (response) {
            
            $scope.preValueSource = response.data;

            preValueSourceResource.getAllPreValueSourceTypesWithSettings()
                .then(function (resp) {
                    $scope.types = resp.data;
                    setTypeAndSettings();
                    getPrevalues();
                    $scope.loaded = true;
                });

            
           
            //set a shared state
            editorState.set($scope.preValueSource);
        });
    }

    $scope.setType = function () {
        $scope.prevalues = null;
        setTypeAndSettings();
    };

    $scope.save = function () {
       
        
        //set settings
        $scope.preValueSource.settings = {};
        angular.forEach($scope.preValueSource.$type.settings, function (setting) {
            var key = setting.alias;
            var value = setting.value;
            $scope.preValueSource.settings[key] = value;
           
        });

        //validate settings
        preValueSourceResource.validateSettings($scope.preValueSource)
            .then(function (response) {

            $scope.errors = response.data;
           
            if ($scope.errors.length > 0) {
                angular.forEach($scope.errors, function(error) {
                   
                    notificationsService.error("Prevaluesource failed to save", error.Message);
                });
            } else {
                //save
                preValueSourceResource.save($scope.preValueSource)
                .then(function (response) {
           
                    $scope.preValueSource = response.data;
                    //set a shared state
                    editorState.set($scope.preValueSource);
                    setTypeAndSettings();
                    getPrevalues();
                    $scope.preValueSourceForm.$dirty = false;
                    navigationService.syncTree({ tree: "prevaluesource", path: [String($scope.preValueSource.id)], forceReload: true });
                    notificationsService.success("Prevaluesource saved", "");
                }, function (err) {
                    notificationsService.error("Prevaluesource failed to save", "");
                });            
            }

            }, function (err) {
                notificationsService.error("Prevaluesource failed to save", "Please check if your settings are valid");
            });
        };

    var setTypeAndSettings = function() {
        $scope.preValueSource.$type = _.where($scope.types, { id: $scope.preValueSource.fieldPreValueSourceTypeId })[0];

        //set settings
        angular.forEach($scope.preValueSource.settings, function (setting) {
            for (var key in $scope.preValueSource.settings) {
                if ($scope.preValueSource.settings.hasOwnProperty(key)) {
                    if (_.where($scope.preValueSource.$type.settings, { alias: key }).length > 0) {
                        _.where($scope.preValueSource.$type.settings, { alias: key })[0].value = $scope.preValueSource.settings[key];
                    }

                }
            }
        });
    };

    var getPrevalues = function() {
        
        preValueSourceResource.getPreValues($scope.preValueSource)
            .then(function (response) {
            $scope.prevalues = response.data;
        });
    };

	});
angular.module("umbraco").controller("UmbracoForms.FormPickerController",
        function ($scope, $http, formResource) {

            $scope.loading = true;
            formResource.getOverView()
                .then(function (response) {
                    $scope.forms  = response.data;
                    $scope.loading = false;
                }, function(err){
                    $scope.error = "An Error has occured while loading!";
                    $scope.loading = false;
                });

            $scope.clear = function () {
                $scope.model.value = null;
            }
        });

function dataSourceResource($http) {

    var apiRoot = "backoffice/UmbracoForms/DataSource/";

    return {

        getScaffold: function (template) {
            return $http.get(apiRoot + "GetScaffold");
        },

        getByGuid: function (id) {
            return $http.get(apiRoot + "GetByGuid?guid=" + id);
        },
        deleteByGuid: function (id) {
            return $http.delete(apiRoot + "DeleteByGuid?guid=" + id);
        },
        save: function (preValueSource) {
            return $http.post(apiRoot + "PostSave", preValueSource);
        },

        validateSettings: function (preValueSource) {
            return $http.post(apiRoot + "ValidateSettings", preValueSource);
        },

        getAllDataSourceTypesWithSettings: function () {
            return $http.get(apiRoot + "GetAllDataSourceTypesWithSettings");
        }
    };
}

angular.module('umbraco.resources').factory('dataSourceResource', dataSourceResource);
function dataSourceWizardResource($http) {

    var apiRoot = "backoffice/UmbracoForms/DataSourceWizard/";

    return {

        getScaffold: function (id) {
            return $http.get(apiRoot + "GetScaffold?guid=" + id);
        },

        getAllFieldTypes: function () {
            return $http.get(apiRoot + "GetAllFieldTypes");
        },

        createForm: function (wizard) {
            return $http.post(apiRoot + "CreateForm", wizard);
        }
    };
}

angular.module('umbraco.resources').factory('dataSourceWizardResource', dataSourceWizardResource);
/**
    * @ngdoc service
    * @name umbraco.resources.dashboardResource
    * @description Handles loading the dashboard manifest
    **/
function exportResource($http) {
    //the factory object returned
    var apiRoot = "backoffice/UmbracoForms/Export/";

    return {

        getExportTypes: function () {
            return $http.get(apiRoot + "GetExportTypes");
        },

        generateExport: function (config) {
            return $http.post(apiRoot + "PostGenerateExport", config);
        },

        getExportUrl: function (token) {
            return apiRoot + "GetExport?token=" + token;
        },

        getExport: function (token) {
            return $http.get(apiRoot + "GetExport?token=" + token);
        }

    };
}

angular.module('umbraco.resources').factory('exportResource', exportResource);

/**
    * @ngdoc service
    * @name umbraco.resources.dashboardResource
    * @description Handles loading the dashboard manifest
    **/
function formResource($http) {
    //the factory object returned
    var apiRoot = "backoffice/UmbracoForms/Form/";

    return {

        getScaffold: function (template) {
            return $http.get(apiRoot + "GetScaffold?template=" + template);
        },

        getAllTemplates: function () {
            return $http.get(apiRoot + "GetFormTemplates");
        },

        getOverView : function(){
                return $http.get(apiRoot + 'GetOverView');
        },

        getByGuid: function (id) {
            return $http.get(apiRoot + "GetByGuid?guid=" + id);
        },

        deleteByGuid: function (id) {
            return $http.delete(apiRoot + "DeleteByGuid?guid=" + id);
        },

        save: function (form) {
            return $http.post(apiRoot + "PostSave", form);
        },

        getAllFieldTypes: function() {
            return $http.get(apiRoot + "GetAllFieldTypes");
        },

        getAllFieldTypesWithSettings: function () {
            return $http.get(apiRoot + "GetAllFieldTypesWithSettings");
        },
        getPrevalueSources: function() {
            return $http.get(apiRoot + "GetPrevalueSources");
        }
    };
}

angular.module('umbraco.resources').factory('formResource', formResource);

/**
    * @ngdoc service
    * @name umbraco.resources.dashboardResource
    * @description Handles loading the dashboard manifest
    **/
function licensingResource($http) {
    //the factory object returned
    var apiRoot = "backoffice/UmbracoForms/Licensing/";

    return {

        getLicenseStatus: function () {
            return $http.get(apiRoot + "GetLicenseStatus");
        },

        getAvailableLicenses: function (config) {
            return $http.post(apiRoot + "PostRetriveAvailableLicenses", config);
        },

        configureLicense: function (config) {
            return $http.post(apiRoot + "PostConfigureLicense", config);
        }

    };
}

angular.module('umbraco.resources').factory('licensingResource', licensingResource);

function pickerResource($http) {

    var apiRoot = "backoffice/UmbracoForms/Picker/";

    return {
        getAllConnectionStrings: function () {
            return $http.get(apiRoot + "GetAllConnectionStrings");
        },
        getAllDataTypes: function () {
            return $http.get(apiRoot + "GetAllDataTypes");
        },
        getAllDocumentTypes: function () {
            return $http.get(apiRoot + "GetAllDocumentTypes");
        },
        getAllDocumentTypesWithAlias: function () {
            return $http.get(apiRoot + "GetAllDocumentTypesWithAlias");
        },
        getAllMediaTypes: function () {
            return $http.get(apiRoot + "GetAllMediaTypes");
        },
        getAllFields: function (formGuid) {
            return $http.get(apiRoot + "GetAllFields?formGuid="+formGuid);
        },
        getAllProperties: function (doctypeAlias) {
            return $http.get(apiRoot + "GetAllProperties?doctypeAlias=" + doctypeAlias);
        }

    };
}

angular.module('umbraco.resources').factory('pickerResource', pickerResource);
function preValueSourceResource($http) {

    var apiRoot = "backoffice/UmbracoForms/PreValueSource/";

    return {

        getScaffold: function (template) {
            return $http.get(apiRoot + "GetScaffold");
        },

        getByGuid: function (id) {
            return $http.get(apiRoot + "GetByGuid?guid=" + id);
        },
        deleteByGuid: function (id) {
            return $http.delete(apiRoot + "DeleteByGuid?guid=" + id);
        },
        save: function (preValueSource) {
            return $http.post(apiRoot + "PostSave", preValueSource);
        },

        validateSettings: function (preValueSource) {
            return $http.post(apiRoot + "ValidateSettings", preValueSource);
        },

        getPreValues: function (preValueSource) {
            return $http.post(apiRoot + "GetPreValues", preValueSource);
        },

        getPreValuesByGuid: function (preValueSourceId) {
            return $http.get(apiRoot + "GetPreValuesByGuid?preValueSourceId=" + preValueSourceId);
        },

        getAllPreValueSourceTypesWithSettings: function () {
            return $http.get(apiRoot + "GetAllPreValueSourceTypesWithSettings");
        }
    };
}

angular.module('umbraco.resources').factory('preValueSourceResource', preValueSourceResource);
/**
    * @ngdoc service
    * @name umbraco.resources.dashboardResource
    * @description Handles loading the dashboard manifest
    **/
function recordResource($http) {
    //the factory object returned
    var apiRoot = "backoffice/UmbracoForms/Record/";

    return {

        getRecords: function (filter) {
            return $http.post(apiRoot + "PostRetriveRecords", filter);
        },

        getRecordsCount: function (filter) {
            return $http.post(apiRoot + "PostRetriveRecordsCount", filter);
        },

        getRecordActions: function () {
            return $http.get(apiRoot + "GetRecordActions");
        },

        getRecordSetActions: function () {
            return $http.get(apiRoot + "GetRecordSetActions");
        },

        executeRecordSetAction : function(model){
            return $http.post(apiRoot + "PostExecuteRecordSetAction", model);
        }

    };
}

angular.module('umbraco.resources').factory('recordResource', recordResource);

/**
    * @ngdoc service
    * @name umbraco.resources.dashboardResource
    * @description Handles loading the dashboard manifest
    **/
function updatesResource($http) {
    //the factory object returned
    var apiRoot = "backoffice/UmbracoForms/Updates/";

    return {
        getUpdateStatus: function () {
            return $http.get(apiRoot + "GetUpdateStatus");
        },

        installLatest: function (version) {
            return $http.get(apiRoot + "InstallLatest?version=" + version);
        }
    };
}

angular.module('umbraco.resources').factory('updatesResource', updatesResource);

function workflowResource($http) {

    var apiRoot = "backoffice/UmbracoForms/Workflow/";

    return {

        getScaffold: function (template) {
            return $http.get(apiRoot + "GetScaffold");
        },

        getByGuid: function (id) {
            return $http.get(apiRoot + "GetByGuid?guid=" + id);
        },
        deleteByGuid: function (id) {
            return $http.delete(apiRoot + "DeleteByGuid?guid=" + id);
        },
        save: function (preValueSource) {
            return $http.post(apiRoot + "PostSave", preValueSource);
        },

        validateSettings: function (preValueSource) {
            return $http.post(apiRoot + "ValidateSettings", preValueSource);
        },

        getAllWorkflowTypesWithSettings: function () {
            return $http.get(apiRoot + "GetAllWorkflowTypesWithSettings");
        },
        getAllWorkflows: function (formGuid) {
            return $http.get(apiRoot + "GetAllWorkflows?formGuid=" + formGuid);
        },
        updateSortOrder: function (state, workflowIds) {
            var data = {};
            data.state = state;
            data.guids = workflowIds;

            return $http.post(apiRoot + "UpdateSortOrder", data);
        },
    };
}

angular.module('umbraco.resources').factory('workflowResource', workflowResource);
angular.module("umbraco.directives")
    .directive('umbFormsAutoFocus', function($timeout) {

        return function(scope, element, attr){

            var update = function() {

                //if it uses its default naming
                if(element.val().indexOf(" field") >= 0){
                    element.focus();
                }

            };

            $timeout(function() {
                update();
            });


            scope.$watch(attr.umbFormsFocusOn, function (_focusVal) {
                $timeout(function () {
                    if (_focusVal) {
                        element.focus();
                        element.select();
                        update();
                    }
                });
            });
    };
});

angular.module("umbraco.directives")
    .directive('umbFormsAutoSize', function($timeout) {

        return function(scope, element, attr){
            var domEl = element[0];
            var update = function(force) {

                if(force === true){
                    element.height(0);
                }

                if(domEl.scrollHeight !== domEl.clientHeight){
                    element.height(domEl.scrollHeight);
                }
            };


            element.bind('keyup keydown keypress change', update);
            element.bind('blur', function(){ update(true); });

            $timeout(function() {
                update();
            });
    };
});

angular.module("umbraco.directives")
    .directive('umbFormsContentPicker', function (dialogService, entityResource, iconHelper) {
    return {
        restrict: 'E',
        replace: true,
        templateUrl: '/App_Plugins/UmbracoForms/directives/umb-forms-content-picker.html',
        require: "ngModel",
        link: function (scope, element, attr, ctrl) {

            ctrl.$render = function() {
                var val = parseInt(ctrl.$viewValue);

                if (!isNaN(val) && angular.isNumber(val) && val > 0) {

                    entityResource.getById(val, "Document").then(function(item) {
                        item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                        scope.node = item;
                    });
                }
            };

            scope.openContentPicker = function () {
                var d = dialogService.treePicker({
                    section: "content",
                    treeAlias: "content",
                    multiPicker: false,
                    callback: populate
                });
            };

            scope.clear = function () {
                scope.id = undefined;
                scope.node = undefined;
                updateModel(0);
            };

            function populate(item) {
                scope.clear();
                item.icon = iconHelper.convertFromLegacyIcon(item.icon);
                scope.node = item;
                scope.id = item.id;
                updateModel(item.id);
            }

            function updateModel(id) {
                ctrl.$setViewValue(id);
                
            }
        }
    };
});
angular.module("umbraco.directives").directive('umbFormsDateRangePicker', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        scope: {
            ngModel: '=',
        },
        template: '<i class="icon icon-calendar"></i> <b class="caret" style="margin-top:9px;"></b> <span></span>',
        link: function (scope, element) {
            $(function () {

                element.daterangepicker(
                   {
                       startDate: moment(scope.ngModel.startDate),
                       endDate: moment(scope.ngModel.endDate),
                       minDate: '01/01/1990',
                       maxDate: moment().format("DD/MM/YYYY"),
                       dateLimit: { days: 6000 },
                       showDropdowns: true,
                       showWeekNumbers: true,
                       timePicker: false,
                       timePickerIncrement: 1,
                       timePicker12Hour: false,
                       ranges: {
                           'Today': [moment(), moment()],
                           'Yesterday': [moment().subtract('days', 1), moment().subtract('days', 1)],
                           'Last 7 Days': [moment().subtract('days', 6), moment()],
                           'Last 30 Days': [moment().subtract('days', 29), moment()],
                           'This Month': [moment().startOf('month'), moment().endOf('month')],
                           'Last Month': [moment().subtract('month', 1).startOf('month'), moment().subtract('month', 1).endOf('month')]
                       },
                       opens: 'right',
                       buttonClasses: ['btn btn-default'],
                       applyClass: 'btn-small btn-primary',
                       cancelClass: 'btn-small',
                       format: 'DD/MM/YYYY',
                       separator: ' to ',
                       locale: {
                           applyLabel: 'Submit',
                           fromLabel: 'From',
                           toLabel: 'To',
                           customRangeLabel: 'Custom Range',
                           daysOfWeek: ['Su', 'Mo', 'Tu', 'We', 'Th', 'Fr', 'Sa'],
                           monthNames: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
                           firstDay: 1
                       }
                   },
                   function (start, end) {

                       //settingsResource.setDateFilter(start.format('YYYY-MM-DD'), end.format('YYYY-MM-DD'));

                       if(angular.isObject(scope.ngModel)){
                           scope.ngModel.startDate = start.format('YYYY-MM-DD');
                           scope.ngModel.endDate = end.format('YYYY-MM-DD');
                       }else{
                            var dateFilter = {};
                            dateFilter.startDate = start.format('YYYY-MM-DD');
                            dateFilter.endDate = end.format('YYYY-MM-DD');
                            scope.ngModel = dateFilter;
                       }


                       scope.$apply();


                       angular.element(element.children()[2]).html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));
                   }
                );

                angular.element(element.children()[2]).html(moment(scope.ngModel.startDate).format('MMMM D, YYYY') + ' - ' + moment(scope.ngModel.endDate).format('MMMM D, YYYY'));

            });
        }
    };
});

angular.module("umbraco.directives")
    .directive('ufDelayedMouseleave', function ($timeout, $parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs, ctrl) {
                var active = false;
                var fn = $parse(attrs.ufDelayedMouseleave);
                element.on("mouseleave", function(event) {
                    var callback = function() {
                        fn(scope, {$event:event});
                    };

                    active = false;
                    $timeout(function(){
                        if(active === false){
                            scope.$apply(callback);
                        }
                    }, 650);
                });

                element.on("mouseenter", function(event, args){
                    active = true;
                });
            }
        };
    });

angular.module("umbraco.directives")
    .directive('umbFormsDesigner', function (dialogService, formService) {
        return {
            scope: {
                form: "=",
                fieldtypes: "=",
                prevaluesources: "="
            },

            transclude: true,

            restrict: 'E',
            replace: true,

            templateUrl: '/App_Plugins/UmbracoForms/directives/umb-forms-designer.html',
            link: function (scope, element, attrs, ctrl) {


                // *********************************************
                // Form management functions
                // *********************************************
                scope.initForm = function(form, fieldtypes){
                    formService.initForm(form, fieldtypes);
                    scope.gotoPageIndex(0);
                };


                // *********************************************
                // Page management functions
                // *********************************************

                scope.gotoPageIndex = function(index){
                    scope.form.$currentPage = scope.form.pages[index];
                    scope.currentPageIndex = index;

                    scope.onFirstPage = scope.currentPageIndex === 0;
                    scope.onLastPage = scope.currentPageIndex === scope.form.pages.length-1;
                };

                scope.addPage = function (form) {
                    scope.closeItemOverlay();
                    formService.addPage(form);
                    scope.gotoPageIndex(form.pages.length -1);
                   
                };

                scope.deletePage = function (page) {
                    var index = scope.form.pages.indexOf(page);
                    scope.form.pages.splice(index, 1);
                    if (index > 0) {
                        scope.currentPageIndex = index - 1;
                        scope.form.$currentPage = scope.form.pages[index - 1];
                    } else {
                        scope.currentPageIndex = 0;
                        scope.form.$currentPage = scope.form.pages[0];
                    }
                    
                    scope.onFirstPage = scope.currentPageIndex === 0;
                    scope.onLastPage = scope.currentPageIndex === scope.form.pages.length - 1;

                    populateFields();
                };

                // *********************************************
                // Fieldset management functions
                // *********************************************

                scope.setCurrentFieldset = function (fieldset) {
                    scope.currentFieldset = fieldset;
                };

                scope.addFieldset = function(page, fieldset, container, index){
                    scope.closeItemOverlay();
                    var _index = page.fieldSets.indexOf(fieldset);
                    formService.addFieldset(page, _index+1);
                };

                scope.deleteFieldset = function(page,fieldset) {
                    formService.deleteFieldset(page, fieldset);
                };

                scope.editFieldset = function (fieldset) {
                    populateFields();
                    dialogService.open(
                    {
                        template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/fieldsetsettings.html",
                        fieldset: fieldset,
                        fields: scope.fields
                    });
                };

                // *********************************************
                // Container management functions
                // *********************************************
                scope.addContainer = function (fieldset, container, index) {
                    scope.closeItemOverlay();
                    var _index = fieldset.containers.indexOf(container);
                    formService.splitContainer(fieldset, container, _index+1);
                };

                // *********************************************
                // Field management functions
                // *********************************************
                scope.initField = function(field){
                    if(field && !field.$fieldType){
                        formService.setFieldType(field,field.fieldTypeId);
                    }
                };

                scope.addField = function (container, fieldtype, index) {
                    scope.closeItemOverlay();
                    formService.addField(container, fieldtype, index+1);
                };

                scope.editField = function(field){
                    populateFields();

                    scope.setFieldType(field);

                    dialogService.open(
                            {
                                template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/fieldsettings.html",
                                field: field,
                                fields: scope.fields,
                                setFieldType: formService.setFieldType,
                                fieldtypes: scope.fieldtypes,
                                prevaluesources: scope.prevaluesources
                            });
                };

                scope.deleteField = function(fieldset,container,field) {
                    formService.deleteField(fieldset,container,field);
                };

                scope.copyField = function (container, field) {
                    var copy ={};
                    angular.copy(field, copy);
                    copy.id = generateGUID();
                    copy.caption = "copy of " + copy.caption;
                    container.fields.splice(container.fields.indexOf(field) + 1, 0, copy);
                    populateFields();
                };

                scope.setFieldType = function (field) {

                    //set settings
                    angular.forEach(field.settings, function (setting) {
                        for (var key in field.settings) {
                            if (field.settings.hasOwnProperty(key)) {
                                if (_.where(field.$fieldType.settings, { alias: key }).length > 0) {
                                    _.where(field.$fieldType.settings, { alias: key })[0].value = field.settings[key];
                                }
                            }
                        }
                    });

                };


                // *********************************************
                // Button/hover state handlers
                // *********************************************
                scope.setCurrentControl = function (field) {
                    scope.currentControl = field;
                };

                scope.setCurrentToolsControl = function (field) {
                    scope.currentToolsControl = field;
                };

                scope.setCurrentRemoveControl = function (Control) {
                    scope.currentRemoveControl = Control;
                };

                scope.setCurrentMoveControl = function (Control) {
                    scope.currentMoveControl = Control;
                };

                scope.setCurrentContainer = function (container) {
                    scope.currentContainer = container;
                };





                // *********************************************
                // Field conditions
                // *********************************************
                scope.addConditionRule = function (condition) {
                    if (!condition.rules){
                        condition.rules = [];
                    }

                    condition.rules.push({
                        field: condition.$newrule.field,
                        operator: condition.$newrule.operator,
                        value: condition.$newrule.value
                    });

                    condition.$newrule.field = null;
                    condition.$newrule.operator = null;
                    condition.$newrule.value = null;
                };

                scope.deleteFieldConditionRule = function (rules, rule) {
                    var index = rules.indexOf(rule);
                    rules.splice(index, 1);
                };


                // *********************************************
                // Sorting configurations
                // *********************************************
                scope.fieldSetSortableOptions = {
                        distance: 10,
                        cursor: "move",
                        placeholder: 'ui-sortable-placeholder',
                        handle: '.cell-tools-move',
                        connectWith: ".umb-forms-fieldsets"
                };

                scope.fieldSortableOptions = {
                    distance: 10,
                    cursor: "move",
                    placeholder: 'ui-sortable-placeholder',
                    handle: '.cell-tools-move',
                    connectWith: ".umb-forms-fields-container"
                };

                // *********************************************
                // Add items overlay menu
                // *********************************************
                scope.overlayMenu = {
                    show: false,
                    style: {},
                    container: undefined,
                    fieldset: undefined
                };

                scope.addItemOverlay = function(sender, field, fieldset, container, index){
                    scope.overlayMenu.container = container;
                    scope.overlayMenu.fieldset = fieldset;
                    scope.overlayMenu.index = index;
                    scope.overlayMenu.style = {};
                    scope.overlayMenu.field = field;

                    //todo calculate position...
                    var offset = $(sender.target).offset();
                    var height = $(window).height();
                    var width = $(window).width();

                    if((height-offset.top) < 250){
                        scope.overlayMenu.style.bottom = 0;
                        scope.overlayMenu.style.top = "initial";
                    }else if(offset.top < 300){
                        scope.overlayMenu.style.top = 190;
                    }

                    scope.overlayMenu.show = true;
                };

                scope.closeItemOverlay = function(){
                    scope.overlayMenu.show = false;
                    scope.overlayMenu.field = undefined;
                };


                scope.adjustSize = function(ev){
                    if(ev.target.scrollHeight > ev.target.clientHeight){
                        $(ev.target).height(ev.target.scrollHeight);
                    }
                };


                // *********************************************
                // Button functons
                // *********************************************

                scope.editForm = function (form, section) {
                    dialogService.open(
                        {
                            template: "/app_plugins/UmbracoForms/Backoffice/Form/dialogs/formsettings.html",
                            form: form,
                            section: section,
                            page: scope.currentPage
                        });
                };

                // *********************************************
                // Internal functons
                // *********************************************
                var populateFields = function() {
                    scope.fields = [];
                    angular.forEach(scope.form.pages, function(page) {
                        angular.forEach(page.fieldSets, function(fieldset) {
                            angular.forEach(fieldset.containers, function(container) {
                                angular.forEach(container.fields, function (field) {
                                    scope.fields.push(field);
                                });
                            });
                        });
                    });
                };

                scope.initForm(scope.form, scope.fieldtypes);
            }
        };
    });

angular.module("umbraco.directives")
    .directive('umbFormsInlinePrevalueEditor', function () {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/UmbracoForms/directives/umb-forms-inline-prevalue-editor.html',
            require: "ngModel",
            link: function (scope, element, attr, ctrl) {
                scope.prevalues = [];

                ctrl.$render = function () {
                    if (Object.prototype.toString.call(ctrl.$viewValue) === '[object Array]') {
                        scope.prevalues = ctrl.$viewValue;
                    }
                };

                scope.addPrevalue = function () {
                    addPrevalue();
                };

                function addPrevalue() {
                   
                    if (scope.prevalues.indexOf(scope.newPrevalue) < 0) {
                        scope.prevalues.push(scope.newPrevalue);
                        scope.newPrevalue = '';
                        updateModel();
                    }
                }
                function updateModel() {
                    ctrl.$setViewValue(scope.prevalues);
                }

                

            }
        };
    });
angular.module("umbraco.directives")
    .directive('umbFormsOverlay', function () {
        return {
            restrict: 'A',
            link: function (scope, el, attrs, ctrl) {
                var margin = 50,
                winHeight = $(window).height(),
                
                calculate = _.throttle(function(){
                    if(el){
                        //detect bottom fold
                        var bottom_dif = (el.offset().top + el.height() + margin) - winHeight;
                        if(bottom_dif > 0){

                            $(el).css('margin-top', function (index, curValue) {
                                return parseInt(curValue, 10) - bottom_dif + 'px';
                            });
                        }else{
                            //else detect top fold           
                        }
                    }
                }, 1000);

                //On resize, make sure to check the overlay
                $(window).bind("resize", function () {
                   winHeight = $(window).height();
                   calculate();
                });
            }
        };
    });
angular.module("umbraco.directives")
    .directive('umbFormsPrevalueEditor', function (dialogService, entityResource, iconHelper) {
        return {
            restrict: 'E',
            replace: true,
            templateUrl: '/App_Plugins/UmbracoForms/directives/umb-forms-prevalue-editor.html',
            require: "ngModel",
            link: function (scope, element, attr, ctrl) {

                scope.prevalues = [];

                ctrl.$render = function () {
                    if (Object.prototype.toString.call(ctrl.$viewValue) === '[object Array]') {
                        scope.prevalues = ctrl.$viewValue;
                    }
                };

                scope.deletePrevalue = function (idx) {
                    scope.prevalues.splice(idx, 1);
                    updateModel();
                };

                scope.addPrevalue = function() {
                    scope.prevalues.push(scope.newPrevalue);
                    scope.newPrevalue = '';
                    updateModel();
                };

                function updateModel() {
                    ctrl.$setViewValue(scope.prevalues);
                }
            }
        };
    });
function formService(preValueSourceResource) {

	var generateGUID = function() {
	    var d = new Date().getTime();

	    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
	        var r = (d + Math.random() * 16) % 16 | 0;
	        d = Math.floor(d / 16);
	        return (c === 'x' ? r : (r & 0x7 | 0x8)).toString(16);
	    });

	    return uuid;
	};


	var service = {
		fieldTypes: [],

		initForm: function(form, fieldtypes){
			service.fieldTypes = fieldtypes;

			if(!form.pages || form.pages.length === 0){
			    service.addPage(form);
			}else{

				_.each(service.getAllFields(form), function(field){

					if(!field.$fieldType){
						service.setFieldType(field,field.fieldTypeId);
					}

				});
			}
		},

		addPage: function(form, index){
			var p = {caption: "", fieldSets: []};
			service.addFieldset(p);

			if(form.pages.length > index){
				form.pages.splice(index, 0, p);
			}else{
				form.pages.push(p);
			}
		},

		addFieldset: function(page, index){
			var fs = {caption: "", containers: [], id:generateGUID()};
			service.addContainer(fs);

			if(page.fieldSets.length > index){
				page.fieldSets.splice(index, 0, fs);
			}else{
				page.fieldSets.push(fs);
			}
		},

		deleteFieldset : function(page, fieldset){
			if(page.fieldSets.length > 1){
				var index = page.fieldSets.indexOf(fieldset);
				page.fieldSets.splice(index, 1);
			}else{
				fieldset.containers.length = 0;
				service.addContainer(fieldset);
			}
		},

		deleteFieldsetAtIndex : function(page, index){
			if(page.fieldSets.length > 1){
				page.fieldSets.splice(index, 1);
			}else{
				fieldset.containers.length = 0;
				service.addContainer(fieldset);
			}
		},

		splitFieldset: function(page, fieldset, container, splitAtIndex){

			var newfieldset = {caption: "", containers: [{ caption: "", fields: [] }] };
			var insertAt = page.fieldSets.indexOf(fieldset);

			page.fieldSets.splice(insertAt+1, 0, newfieldset);

			var oldFields = container.fields.slice(0,splitAtIndex+1);
			var newFields = container.fields.slice(splitAtIndex+1);

			newfieldset.containers[0].fields = newFields;
			container.fields = oldFields;
		},

		addContainer: function(fieldset, index){
			var c = { caption: "", fields: [] };

			if(fieldset.containers.length > index){
				fieldset.containers.splice(index, 0, c);
			}else{
				fieldset.containers.push(c);
			}
		},

		splitContainer: function(fieldset, container, splitAtIndex){

			var newContainer = { caption: "", fields: [] };
			var insertAt = fieldset.containers.indexOf(container);

			fieldset.containers.splice(insertAt-1, 0, newContainer);
			var newFields = container.fields.slice(0,splitAtIndex+1);
			var oldFields = container.fields.slice(splitAtIndex+1);

			newContainer.fields = newFields;
			container.fields = oldFields;
		},

		deleteContainer: function(fieldset, container){
			//only delete the container if there are multiple ones on this fieldseet
			//otherwise keep it and just clear its contents
			if(fieldset.containers.length > 1){
				var index = fieldset.containers.indexOf(container);
				if(index >= 0){
					service.deleteContainerAtIndex(fieldset, index);
				}
			}else{
				container.fields.length = 0;
			}
		},

		deleteContainerAtIndex: function(fieldset, index){

			if(fieldset.containers.length > 1){
				fieldset.containers.splice(index, 1);
			}else{
				fieldset.containers.length = 0;
			}
		},


		syncContainerWidths : function(form){
			_.each(form.pages, function(page){
				_.each(page.fieldSets, function(fieldset){
					var containers = fieldset.containers.length;
					var avrg = Math.floor(12 / containers);
					_.each(fieldset.containers, function(container){
						container.width = avrg;
					});
				});
			});
		},

		addField : function(container, fieldtype, index) {
		    var newField = {
		        caption: fieldtype.name + " field",
		        id: generateGUID(),
		        settings: {},
		        preValues: [],
		        $focus: true
		};

			service.loadFieldTypeSettings(newField, fieldtype);

			if(container.fields.length > index){
				container.fields.splice(index, 0, newField);
			}else{
				container.fields.push(newField);
			}

		},

		getAllFields : function(form){
			var fields = [];
			if(form.pages){
				_.each(form.pages, function(page){
					if(page.fieldSets){
						_.each(page.fieldSets, function(fieldset){
							if(fieldset.containers){
								_.each(fieldset.containers, function(container){
									if(container.fields){
										_.each(container.fields, function(field){
											fields.push(field);
										});
									}
								});
							}
						});
					}
				});
			}

			return fields;
		},


		deleteField: function(fieldset,container,field){
			var index = container.fields.indexOf(field);
			if(index >= 0){
				service.deleteFieldAtIndex(fieldset, container, index);
			}
		},


		deleteFieldAtIndex: function(fieldset,container,index){

			container.fields.splice(index, 1);

			if(container.fields.length === 0){
				service.deleteContainer(fieldset,container);
			}

		},


		setFieldType: function(field, fieldTypeId){
			//get field type
			field.fieldTypeId = fieldTypeId;

			var fldt = _.find(service.fieldTypes, function(ft){return ft.id === field.fieldTypeId; });
			field.$fieldType = fldt;

			service.loadFieldTypeSettings(field, field.$fieldType);

			
			service.loadFieldTypePrevalues(field);
			
		},

        loadFieldTypePrevalues : function(field) {
            
            if (field.prevalueSourceId !== null && field.prevalueSourceId !== "00000000-0000-0000-0000-000000000000") {

                preValueSourceResource.getPreValuesByGuid(field.prevalueSourceId)
                    .then(function(response) {
                        field.$preValues = response.data;

                    });
            } else {
                field.$preValues = null;
            }
           
        },

		loadFieldTypeSettings : function(field, fieldtype){

			var stng = angular.copy(fieldtype.settings);

			if(field.fieldTypeId !== fieldtype.id){
				field.settings = {};
			}

			field.fieldTypeId = fieldtype.id;
			field.$fieldType = fieldtype;

			if(fieldtype.settings){
				_.each(fieldtype.settings, function(setting){
					if(!field.settings[setting.alias]){
						field.settings[setting.alias] = "";
					}
				});
			}
		},


		deleteConditionRule : function(rules, rule) {
		    var index = rules.indexOf(rule);
		    rules.splice(index, 1);
		},

        addConditionRule : function(condition) {
	        if (!condition.rules) {
	            condition.rules = [];
	        }

	        condition.rules.push({
	            field: condition.$newrule.field,
	            operator: condition.$newrule.operator,
	            value: condition.$newrule.value
	        });
        }

	};

	return service;
}
angular.module('umbraco.services').factory('formService', formService);
