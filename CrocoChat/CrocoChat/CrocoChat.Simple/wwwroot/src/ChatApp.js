var ChatApp = /** @class */ (function () {
    function ChatApp() {
    }
    /*
     *
     * */
    ChatApp.prototype.ShowLoginModal = function () {
        $("#loginModal").modal('show');
    };
    ChatApp.prototype.Login = function () {
        var data;
    };
    return ChatApp;
}());
var App = new ChatApp();
