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