"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/productHub").build();

$(function () {
    connection.start().then(function () {
        console.log("Connected!");
        
    }).catch(function (err) {
        return console.error(err.toString());
    });
});

connection.on("ReceiveProducts", function (response) {
    var currentPriceListId = $('#priceListId').val();

    if (currentPriceListId != response.priceListId) {
        console.log("currentPriceListId is not equal to response.priceListId");
        return;
    }

    var routeValues = window.location.search.substring(1).split('&').reduce((obj, item) => {
        let [key, value] = item.split('=');
        obj[key.toLocaleLowerCase()] = value;
        return obj
    }, {})

    $.ajax({
        url: window.location.origin + `/PriceLists/DetailsRaw/${currentPriceListId}`,
        type: 'GET',
        data: routeValues,
        success: function (data) {
            //console.log(data);
            fillData(data);
        },
        error: function (xhr, status, error) {
            console.log(xhr.responseText);
        }
    });
});

function fillData(data) {
    var tableBody = $('#tableBody');
    tableBody.empty();
    var tableHead = $('#tableHead');

    var customColumns = tableHead.find('th[class^="product-column-"]');
    
    data.products.forEach(function (product) {
        var row = $('<tr></tr>');
        row.append($('<input>').attr({
            type: 'hidden',
            name: 'productId',
            value: product.productId
        }));
        row.append($('<td></td>').text(product.productName));
        row.append($('<td></td>').text(product.article));

        customColumns.each(function () {
            var columnId = $(this).find('input[name="ProductColumnId"]').val();
            row.append($('<td></td>').text(product.columns.find(x => x.columnId == columnId).value));
        });

        var tdDeleteButton = $('<td></td>');
        tdDeleteButton.append($('<button></button>').attr({
            type: 'button',
            class: 'btn btn-sm btn-danger',
            onclick: `showModalDeleteProduct(${product.productId})`
        }).text('Удалить'));
        row.append(tdDeleteButton);

        tableBody.append(row);
    });
}