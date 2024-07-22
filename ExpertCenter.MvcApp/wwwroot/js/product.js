"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/productHub").build();

$(function () {
    connection.start().then(function () {
        console.log("Connected!");
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

connection.on("ReceiveProducts", function (products) {
    console.log(products);
});