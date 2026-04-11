import { API, APIError } from "./Utility/API.js";
import { hash } from "./Utility/Hash.js";
const registerButton = document.getElementById("register");
const loginButton = document.getElementById("login");
const errorField = document.getElementById("errorText");
const usernameField = document.getElementById("username");
const passwordField = document.getElementById("password");
registerButton.addEventListener("click", () => onButtonPressed(Action.Register));
loginButton.addEventListener("click", () => onButtonPressed(Action.Login));
var Action;
(function (Action) {
    Action[Action["Login"] = 0] = "Login";
    Action[Action["Register"] = 1] = "Register";
})(Action || (Action = {}));
async function getAuth() {
    const auth = {
        username: usernameField.value,
        password: passwordField.value,
        passwordHash: await hash(passwordField.value)
    };
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
async function onButtonPressed(action) {
    const auth = await getAuth();
    if (!auth)
        return;
    try {
        if (action === Action.Login)
            await API.get(`/api/users/${auth.username}`, new Headers({ "username": auth.username, "passwordHash": auth.passwordHash }));
        else if (action === Action.Register)
            await API.post("/api/users/", new Headers(), auth);
    }
    catch (error) {
        if (error instanceof APIError) {
            if (error.statusCode === 401)
                errorField.innerText = error.serverMessage;
            else
                errorField.innerText = "Server error! Please try again.";
            // return;
        }
        else
            throw error;
    }
    sessionStorage.setItem("username", auth.username);
    sessionStorage.setItem("password", auth.password);
    window.location.replace("/dnsblocker.html");
}
