var ToastrWorker = /** @class */ (function () {
    function ToastrWorker() {
    }
    ToastrWorker.prototype.ShowError = function (text) {
        var data = {
            IsSucceeded: false,
            Message: text
        };
        this.HandleBaseApiResponse(data);
    };
    ToastrWorker.prototype.ShowSuccess = function (text) {
        var data = {
            IsSucceeded: true,
            Message: text
        };
        this.HandleBaseApiResponse(data);
    };
    ToastrWorker.prototype.HandleBaseApiResponse = function (data) {
        if (data.IsSucceeded === undefined || data.Message === undefined) {
            alert("Произошла ошибка. Объект не является типом BaseApiResponse");
            return;
        }
        toastr.options = {
            "closeButton": false,
            "debug": false,
            "newestOnTop": false,
            "progressBar": false,
            "positionClass": "toast-top-right",
            "preventDuplicates": false,
            "onclick": null,
            "showDuration": "300",
            "hideDuration": "1000",
            "timeOut": "5000",
            "extendedTimeOut": "1000",
            "showEasing": "swing",
            "hideEasing": "linear",
            "showMethod": "fadeIn",
            "hideMethod": "fadeOut"
        };
        if (data.IsSucceeded) {
            toastr.success(data.Message);
        }
        else {
            toastr.error(data.Message);
        }
    };
    return ToastrWorker;
}());

var Requester_Resx = /** @class */ (function () {
    function Requester_Resx() {
        this.YouPassedAnEmtpyArrayOfObjects = "Вы подали пустой объект в запрос";
        this.ErrorOccuredWeKnowAboutIt = "Произошла ошибка! Мы уже знаем о ней, и скоро с ней разберемся!";
        this.FilesNotSelected = "Файлы не выбраны";
    }
    return Requester_Resx;
}());
var Requester = /** @class */ (function () {
    function Requester() {
    }
    Requester.prototype.SendPostRequestWithAnimation = function (link, data, onSuccessFunc, onErrorFunc) {
        Requester.SendAjaxPostInner(link, data, onSuccessFunc, onErrorFunc);
    };
    Requester.prototype.Get = function (link, data, onSuccessFunc, onErrorFunc) {
        var params = {
            type: "GET",
            data: data,
            url: link,
            async: true,
            cache: false,
            success: (function (response) {
                if (onSuccessFunc) {
                    onSuccessFunc(response);
                }
            }).bind(this),
            error: (function (jqXHR, textStatus, errorThrown) {
                //Вызываю внешнюю функцию-обработчик
                if (onErrorFunc) {
                    onErrorFunc(jqXHR, textStatus, errorThrown);
                }
            }).bind(this)
        };
        $.ajax(params);
    };
    Requester.SendAjaxPostInner = function (link, data, onSuccessFunc, onErrorFunc) {
        if (data == null) {
            data = {};
        }
        var params = {};
        params.type = "POST";
        params.data = data;
        params.url = link;
        params.async = true;
        params.cache = false;
        params.success = (function (response) {
            if (onSuccessFunc) {
                onSuccessFunc(response);
            }
        }).bind(this);
        params.error = (function (jqXHR, textStatus, errorThrown) {
            //Вызываю внешнюю функцию-обработчик
            if (onErrorFunc) {
                onErrorFunc(jqXHR, textStatus, errorThrown);
            }
        }).bind(this);
        var isArray = data.constructor === Array;
        if (isArray) {
            params.contentType = "application/json; charset=utf-8";
            params.dataType = "json";
            params.data = JSON.stringify(data);
        }
        $.ajax(params);
    };
    Requester.prototype.Post = function (link, data, onSuccessFunc, onErrorFunc) {
        Requester.SendAjaxPostInner(link, data, onSuccessFunc, onErrorFunc);
    };
    Requester.Resources = new Requester_Resx();
    return Requester;
}());

/// <reference path="../../node_modules/@microsoft/signalr/dist/esm/browser-index.d.ts"/>
/// <reference path="../../node_modules/@types/bootstrap/index.d.ts"/>
function ErrorHandler(err) {
    console.error(err.toString());
}
function GetValue(id) {
    return document.getElementById(id).value;
}
$(function () {
    //Cоздаем объект подключения - cartHub
    var appHub = new signalR.HubConnectionBuilder()
        .withUrl("/appHub")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();
    appHub.on("onProductLikeRemoved", function (data) {
    });
    //Открываем соединение
    appHub.start().then(function () {
        appHub.invoke("getCart", {
            FromDataBase: false
        }).catch(ErrorHandler);
    });
    $("#register-btn").on("click", function () {
        var data = {
            Name: GetValue("register-name"),
            LastName: GetValue("register-name"),
            Email: GetValue("register-email"),
            Password: GetValue("register-pass")
        };
        appHub.invoke("register", data).catch(ErrorHandler);
    });
    $(document).on("click", ".some-class", function (e) {
        var data = {
            Prop: e.currentTarget.getAttribute("some-attr")
        };
        appHub.invoke("removeCartItem", data).catch(ErrorHandler);
    });
});

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