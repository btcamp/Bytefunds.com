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