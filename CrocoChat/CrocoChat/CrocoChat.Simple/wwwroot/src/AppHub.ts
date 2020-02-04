/// <reference path="../../node_modules/@microsoft/signalr/dist/esm/browser-index.d.ts"/>
/// <reference path="../../node_modules/@types/bootstrap/index.d.ts"/>

function ErrorHandler(err) {
    console.error(err.toString());
}


function GetValue(id: string) : string {
    return (document.getElementById(id) as HTMLInputElement).value;
}

$(function () {

    //Cоздаем объект подключения - cartHub
    const appHub = new signalR.HubConnectionBuilder()
        .withUrl("/appHub")
        .withAutomaticReconnect()
        .configureLogging(signalR.LogLevel.Information)
        .build();

    appHub.on("onProductLikeRemoved", (data: object): void => {

    });


    //Открываем соединение
    appHub.start().then(() => {

        appHub.invoke("getCart", {
            FromDataBase: false
        }).catch(ErrorHandler);

    });


    $("#register-btn").on("click", () => {

        let data = {
            Name: GetValue("register-name"),
            LastName: GetValue("register-name"),
            Email: GetValue("register-email"),
            Password: GetValue("register-pass")
        };

        appHub.invoke("register", data).catch(ErrorHandler);
    });



    $(document).on("click", ".some-class", function (e) {
        const data = {
            Prop: (e.currentTarget as HTMLElement).getAttribute("some-attr")
        }

        appHub.invoke("removeCartItem", data).catch(ErrorHandler);
    });

});