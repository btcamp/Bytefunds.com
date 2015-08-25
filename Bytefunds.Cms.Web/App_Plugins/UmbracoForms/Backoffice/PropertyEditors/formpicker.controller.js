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
