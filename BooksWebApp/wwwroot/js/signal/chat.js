//"use strict";

/*var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .withAutomaticReconnect()
    .build();*/

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var dt = document.createElement("dt");
    dt.textContent = user;
    document.getElementById("messagesList").appendChild(dt);

    var dd = document.createElement("dd");
    dd.textContent = msg;
    document.getElementById("messagesList").appendChild(dd);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {

    var user = $("#userNameLeft").data("name");
    var message = $("#messageInput").val();
    connection
        .invoke("SendMessage", user, message)
        .catch(function (err) {
            return console.error(err.toString());
        });
    $("#messageInput").val('');
    event.preventDefault();
});