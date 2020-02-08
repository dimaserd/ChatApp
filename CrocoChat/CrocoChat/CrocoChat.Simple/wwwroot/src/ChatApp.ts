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



enum LoginResult {
    Error = <any>'Error',
    AlreadyAuthenticated = <any>'AlreadyAuthenticated',
    UnSuccessfulAttempt = <any>'UnSuccessfulAttempt',
    EmailNotConfirmed = <any>'EmailNotConfirmed',
    SuccessfulLogin = <any>'SuccessfulLogin',
    NeedConfirmation = <any>'NeedConfirmation',
    UserDeactivated = <any>'UserDeactivated'
}



class ChatApp {

    Requeter: Requester = new Requester();
    ToastrWorker: ToastrWorker = new ToastrWorker();

    constructor() {
        this.SetHandlers();
        this.IsAuthenticated();
    }

    MakeNoMesages() {
        $(".chat-footer").fadeOut();
        $(".messages").fadeOut();
        $(".chat-header").fadeOut();
        $(".no-message-container").fadeIn();
        document.getElementById("chats").classList.remove("active");
    }

    ShowMessages() {
        $(".chat-footer").fadeIn();
        $(".messages").fadeIn();
        $(".chat-header").fadeIn();
        $(".no-message-container").fadeOut();
    }

    OpenSideBar() {
        document.getElementsByClassName("sidebar-group")[0].classList.add("mobile-open")
    }

    OpenMainSideBar() {
        document.getElementById("main-side-bar").classList.add("mobile-open");
    }

    ShowContactInfo() {
        document.getElementById("profile-sidebar").classList.add("mobile-open");
        document.getElementById("contact-information").classList.add("active");
    }

     ShowLoginModal() {
        $("#loginModal").modal('show');
    }

    GetValueById(elementName: string): string {
        return (document.getElementById(elementName) as HTMLInputElement).value;
    }

    SetHandlers(): void {
        document.getElementById("login-btn").addEventListener("click", this.Login.bind(this));
        document.getElementById("register-btn").addEventListener("click", this.Register.bind(this));
    }

    IsAuthorizedHandler(x: AuthorizedResponse) {

        console.log("IsAuthorizedHandler", x);

        if (!x.IsAuthenticated) {
            this.ShowLoginModal();
        }
    }

    IsAuthenticated(): void {
        this.Requeter.Post('/Api/Account/IsAuthorized', {}, this.IsAuthorizedHandler.bind(this), null);
    }

    RegisterHandler(x: IBaseApiResponse) {
        this.ToastrWorker.HandleBaseApiResponse(x);

        if (x.IsSucceeded) {
            this.HideModals();
        }
    }

    Register(): void {
        let data: RegisterModel = {
            Email: this.GetValueById("register-email"),
            LastName: this.GetValueById("register-lastname"),
            Name: this.GetValueById("register-name"),
            Password: this.GetValueById("register-pass")
        };

        this.Requeter.Post('/Api/Account/RegisterAndSignIn', data, this.RegisterHandler.bind(this), null);
    }

    LoginHandler(x: IGenericBaseApiResponse<LoginResultModel>): void {

        console.log("LoginHandler", x);

        this.ToastrWorker.HandleBaseApiResponse(x);

        if (x.IsSucceeded) {
            this.HideModals();
        }
    }

    public HideModals(): void {

        $('.modal').modal('hide');

        $(".modal-backdrop.fade").remove();

        $('.modal').on('shown.bs.modal', () => {
        })
    }

    Login(): void {
        let data: LoginModel = {
            Email: this.GetValueById("login-username"),
            Password: this.GetValueById("login-password")
        };

        this.Requeter.Post('/Api/Account/Login', data, this.LoginHandler.bind(this), null);
    }
}

var App = new ChatApp();