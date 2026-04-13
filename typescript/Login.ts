import { API, APIError } from "./Utility/API.js";
import { hash } from "./Utility/Hash.js";

const registerButton = document.getElementById("register")!;
const loginButton = document.getElementById("login")!;
const errorField = document.getElementById("errorText")!;
const usernameField = document.getElementById("username") as HTMLInputElement;
const passwordField = document.getElementById("password") as HTMLInputElement;

registerButton.addEventListener("click", () => onButtonPressed(Action.Register));
loginButton.addEventListener("click", () => onButtonPressed(Action.Login));

interface Auth {
    username: string;
    password: string;
    passwordHash: string;
}

enum Action {
    Login,
    Register
}

async function getAuth(): Promise<Auth | null> {
    const auth: Auth = {
        username: usernameField.value.toLowerCase(),
        password: passwordField.value,
        passwordHash: await hash(passwordField.value)
    }

    errorField.innerText = "";

    if (auth.username === "") {
        errorField.innerText = "Please enter your username";
        return null;
    }

    if (auth.password === "") {
        errorField.innerText = "Please enter your password";
        return null;
    }

    return auth;
}

async function onButtonPressed(action: Action): Promise<void> {
    const auth = await getAuth();
    if (!auth) return;

    try {
        if (action === Action.Login)
            await API.get(`/api/users/${auth.username}`, new Headers({ "username": auth.username, "passwordHash": auth.passwordHash }));
        else if (action === Action.Register)
            await API.post("/api/users/", new Headers(), { username: auth.username, passwordHash: auth.passwordHash });
    }
    catch (error) {
        if (error instanceof APIError) {
            if (error.statusCode === 401 && action == Action.Login)
                errorField.innerText = "Invalid username or password!";
            else if (error.statusCode === 400 && action == Action.Register)
                errorField.innerText = "You cannot create an account with this username.";
            else
                errorField.innerText = "Server or internal error! Please try again."
            return;
        }
        else
            throw error;
    }

    sessionStorage.setItem("username", auth.username);
    sessionStorage.setItem("password", auth.password);
    window.location.replace("/dnsblocker.html");
}
