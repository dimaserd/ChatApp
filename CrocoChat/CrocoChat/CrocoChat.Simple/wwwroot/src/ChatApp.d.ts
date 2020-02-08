interface IBaseApiResponse {
    IsSucceeded: boolean;
    Message: string;
}
interface IGenericBaseApiResponse<T> extends IBaseApiResponse {
    ResponseObject: T;
}
interface RegisterModel {
    Email: string;
    Name: string;
    LastName: string;
    Password: string;
}
interface AuthorizedResponse {
    IsAuthenticated: boolean;
    UserId: string;
}
interface LoginModel {
    Email: string;
    Password: string;
}
interface LoginResultModel {
    Result: LoginResult;
    TokenId: string;
}
declare enum LoginResult {
    Error,
    AlreadyAuthenticated,
    UnSuccessfulAttempt,
    EmailNotConfirmed,
    SuccessfulLogin,
    NeedConfirmation,
    UserDeactivated
}
declare class ChatApp {
    Requeter: Requester;
    ToastrWorker: ToastrWorker;
    constructor();
    MakeNoMesages(): void;
    ShowMessages(): void;
    OpenSideBar(): void;
    OpenMainSideBar(): void;
    ShowContactInfo(): void;
    ShowLoginModal(): void;
    GetValueById(elementName: string): string;
    SetHandlers(): void;
    IsAuthorizedHandler(x: AuthorizedResponse): void;
    IsAuthenticated(): void;
    RegisterHandler(x: IBaseApiResponse): void;
    Register(): void;
    LoginHandler(x: IGenericBaseApiResponse<LoginResultModel>): void;
    HideModals(): void;
    Login(): void;
}
declare var App: ChatApp;
