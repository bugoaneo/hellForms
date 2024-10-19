angular.module("umbraco")
    .controller("SportPriority.ThemeSelect", function ($scope, themeSelectResources, editorService) {
        if (typeof $scope.model.value === "undefined" || typeof $scope.model.value !== "object") {
            $scope.model.value = {
                websiteBaseTheme: ""
            };
        }

        themeSelectResources.getAllThemes().then(function (data) {
            $scope.allThemes = data.data;
        }, function (error) {
            console.error(error);
        });


        $scope.openEditor = function () {
            var options = {
                title: "Копировать тему",
                view: "/App_Plugins/ThemeSelect/themeSelectEditor.html?v=2",
                value: $scope.model.value,
                size: "small",
                submit: function (data) {
                    $scope.allThemes.push(data);
                    console.log($scope.allThemes);
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            };
            editorService.open(options);
        };
    });

angular.module("umbraco").controller("ThemeSelect", function ($scope, themeSelectResources, editorService, notificationsService) {
    $this = this;

    var srcdata = $scope.model.value;
    $this.data = {
        websiteBaseTheme: srcdata.websiteBaseTheme,
        newThemeName: null
    };
    themeSelectResources.getAllThemes().then(function (data) {
        $this.data.allThemes = data.data;
    }, function (error) {
        console.error(error);
    });

    this.submit = function () {
        themeSelectResources.copyDirectory($this.data.websiteBaseTheme, $this.data.newThemeName).then(function (data) {
            if ($scope.model.submit) {
                $scope.model.submit($this.data.newThemeName);
                notificationsService.success(data.data);
            }
        }, function (error) {
            console.error(error);
            notificationsService.error(error.data.Message);
        });
    };

    this.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        }
    };
});
