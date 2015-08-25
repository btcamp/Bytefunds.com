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
