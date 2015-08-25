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
