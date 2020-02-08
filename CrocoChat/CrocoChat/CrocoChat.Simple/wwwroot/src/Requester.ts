class Requester_Resx {
    YouPassedAnEmtpyArrayOfObjects: string = "Вы подали пустой объект в запрос";
    ErrorOccuredWeKnowAboutIt: string = "Произошла ошибка! Мы уже знаем о ней, и скоро с ней разберемся!";
    FilesNotSelected: string = "Файлы не выбраны";
}

class Requester {

    static Resources: Requester_Resx = new Requester_Resx();

    SendPostRequestWithAnimation<TObject>(link: string, data: Object, onSuccessFunc: (x: TObject) => void, onErrorFunc: Function): void {
        Requester.SendAjaxPostInner(link, data, onSuccessFunc, onErrorFunc);
    }

    Get<TObject>(link: string, data: Object, onSuccessFunc: (x: TObject) => void, onErrorFunc: Function) {

        const params = {
            type: "GET",
            data: data,
            url: link,
            async: true,
            cache: false,
            success: (response => {
                
                if (onSuccessFunc) {
                    onSuccessFunc(response);
                }
            }).bind(this),

            error: ((jqXHR, textStatus, errorThrown) => {
                
                //Вызываю внешнюю функцию-обработчик
                if (onErrorFunc) {
                    onErrorFunc(jqXHR, textStatus, errorThrown);
                }

            }).bind(this)
        };

        $.ajax(params);
    }


    static SendAjaxPostInner(link: string, data: Object, onSuccessFunc: Function, onErrorFunc: Function) {

        if (data == null) {
            data = {};
        }

        let params: any = {};

        params.type = "POST";
        params.data = data;
        params.url = link;
        params.async = true;
        params.cache = false;
        params.success = (response => {

            if (onSuccessFunc) {
                onSuccessFunc(response);
            }
        }).bind(this);

        params.error = ((jqXHR, textStatus, errorThrown) => {
            
            //Вызываю внешнюю функцию-обработчик
            if (onErrorFunc) {
                onErrorFunc(jqXHR, textStatus, errorThrown);
            }

        }).bind(this);

        const isArray = data.constructor === Array;

        if (isArray) {
            params.contentType = "application/json; charset=utf-8";
            params.dataType = "json";
            params.data = JSON.stringify(data);
        }

        $.ajax(params);
    }

    Post<TObject>(link: string, data: Object, onSuccessFunc: (x: TObject) => void, onErrorFunc: Function) {
        Requester.SendAjaxPostInner(link, data, onSuccessFunc, onErrorFunc);
    }
}