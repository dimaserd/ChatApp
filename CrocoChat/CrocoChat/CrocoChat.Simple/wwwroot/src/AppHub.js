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
