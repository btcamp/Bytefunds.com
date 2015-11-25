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