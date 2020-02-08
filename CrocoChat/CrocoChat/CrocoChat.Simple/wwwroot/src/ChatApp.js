var LoginResult;
(function (LoginResult) {
    LoginResult[LoginResult["Error"] = 'Error'] = "Error";
    LoginResult[LoginResult["AlreadyAuthenticated"] = 'AlreadyAuthenticated'] = "AlreadyAuthenticated";
    LoginResult[LoginResult["UnSuccessfulAttempt"] = 'UnSuccessfulAttempt'] = "UnSuccessfulAttempt";
    LoginResult[LoginResult["EmailNotConfirmed"] = 'EmailNotConfirmed'] = "EmailNotConfirmed";
    LoginResult[LoginResult["SuccessfulLogin"] = 'SuccessfulLogin'] = "SuccessfulLogin";
    LoginResult[LoginResult["NeedConfirmation"] = 'NeedConfirmation'] = "NeedConfirmation";
    LoginResult[LoginResult["UserDeactivated"] = 'UserDeactivated'] = "UserDeactivated";
})(LoginResult || (LoginResult = {}));
var ChatApp = /** @class */ (function () {
    function ChatApp() {
        this.Requeter = new Requester();
        this.ToastrWorker = new ToastrWorker();
        this.SetHandlers();
        this.IsAuthenticated();
    }
    ChatApp.prototype.MakeNoMesages = function () {
        $(".chat-footer").fadeOut();
        $(".messages").fadeOut();
        $(".chat-header").fadeOut();
        $(".no-message-container").fadeIn();
        document.getElementById("chats").classList.remove("active");
    };
    ChatApp.prototype.ShowMessages = function () {
        $(".chat-footer").fadeIn();
        $(".messages").fadeIn();
        $(".chat-header").fadeIn();
        $(".no-message-container").fadeOut();
    };
    ChatApp.prototype.OpenSideBar = function () {
        document.getElementsByClassName("sidebar-group")[0].classList.add("mobile-open");
    };
    ChatApp.prototype.OpenMainSideBar = function () {
        document.getElementById("main-side-bar").classList.add("mobile-open");
    };
    ChatApp.prototype.ShowContactInfo = function () {
        document.getElementById("profile-sidebar").classList.add("mobile-open");
        document.getElementById("contact-information").classList.add("active");
    };
    ChatApp.prototype.ShowLoginModal = function () {
        $("#loginModal").modal('show');
    };
    ChatApp.prototype.GetValueById = function (elementName) {
        return document.getElementById(elementName).value;
    };
    ChatApp.prototype.SetHandlers = function () {
        document.getElementById("login-btn").addEventListener("click", this.Login.bind(this));
        document.getElementById("register-btn").addEventListener("click", this.Register.bind(this));
    };
    ChatApp.prototype.IsAuthorizedHandler = function (x) {
        console.log("IsAuthorizedHandler", x);
        if (!x.IsAuthenticated) {
            this.ShowLoginModal();
        }
    };
    ChatApp.prototype.IsAuthenticated = function () {
        this.Requeter.Post('/Api/Account/IsAuthorized', {}, this.IsAuthorizedHandler.bind(this), null);
    };
    ChatApp.prototype.RegisterHandler = function (x) {
        this.ToastrWorker.HandleBaseApiResponse(x);
        if (x.IsSucceeded) {
            this.HideModals();
        }
    };
    ChatApp.prototype.Register = function () {
        var data = {
            Email: this.GetValueById("register-email"),
            LastName: this.GetValueById("register-lastname"),
            Name: this.GetValueById("register-name"),
            Password: this.GetValueById("register-pass")
        };
        this.Requeter.Post('/Api/Account/RegisterAndSignIn', data, this.RegisterHandler.bind(this), null);
    };
    ChatApp.prototype.LoginHandler = function (x) {
        console.log("LoginHandler", x);
        this.ToastrWorker.HandleBaseApiResponse(x);
        if (x.IsSucceeded) {
            this.HideModals();
        }
    };
    ChatApp.prototype.HideModals = function () {
        $('.modal').modal('hide');
        $(".modal-backdrop.fade").remove();
        $('.modal').on('shown.bs.modal', function () {
        });
    };
    ChatApp.prototype.Login = function () {
        var data = {
            Email: this.GetValueById("login-username"),
            Password: this.GetValueById("login-password")
        };
        this.Requeter.Post('/Api/Account/Login', data, this.LoginHandler.bind(this), null);
    };
    return ChatApp;
}());
var App = new ChatApp();
