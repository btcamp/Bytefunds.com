
function PaymentClear($scope, $http, navigationService, notificationsService) {
    $scope.cancel = function () {
        navigationService.hideDialog();
    }
    $scope.clear = function () {
        $http.post("/umbraco/Api/Payment/Clear").success(function (result) {
            if (result.Success) {
                notificationsService.success(result.Msg);
            }
            else {
                notificationsService.error("清除失败，请重试！");
            }
        }).error(function () {
            notificationsService.error("网路错误，请重试！");
        });
    }
}

angular.module("umbraco").controller("PaymentClear", PaymentClear);


function WithdrawController($scope, $http, navigationService, notificationsService) {
    $scope.loaded = true;
    $scope.model = {
        name: $scope.currentNode.name,
        id: $scope.currentNode.id,
        amount: $scope.currentNode.metaData.amount
    }

    $scope.save = function () {
        $scope.loaded = false;
        $http.post("/umbraco/Api/Withdraw/Approved", { id: $scope.model.id }).success(function (result) {
            $scope.loaded = true;
            if (result.Success) {
                notificationsService.success(result.Msg);
                navigationService.hideDialog();
            }
            else {
                notificationsService.error(result.Msg);
            }
        }).error(function () {
            $scope.loaded = true;
            notificationsService.error("网络错误，请重试！");
        });
    }
}
angular.module("umbraco").controller("WithdrawController", WithdrawController);