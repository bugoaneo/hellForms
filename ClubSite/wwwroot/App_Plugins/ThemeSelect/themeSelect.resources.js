angular.module("umbraco.resources").factory("themeSelectResources",
    function ($http) {
        return {
            getAllThemes: function () {
                return $http.get("/umbraco/backoffice/theme/ThemeSelect/GetAllThemes");
            },
            copyDirectory: function (oldFolderName, newFolderName) {
                return $http.post("/umbraco/backoffice/theme/ThemeSelect/CopyDirectory", { "OldFolderName" : oldFolderName, "NewFolderName" : newFolderName });
            }
        };
    });
