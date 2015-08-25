
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