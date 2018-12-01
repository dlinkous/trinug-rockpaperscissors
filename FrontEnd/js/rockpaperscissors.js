$(document).ready(handleDocumentReady);

function handleDocumentReady() {
    $("#beginRock").click(function () {
        var request = { Shape: "Rock" };
        beginGame(request);
    });
    $("#beginPaper").click(function () {
        var request = { Shape: "Paper" };
        beginGame(request);
    });
    $("#beginScissors").click(function () {
        var request = { Shape: "Scissors" };
        beginGame(request);
    });
    $("#finishRock").click(function () {
        var request = { Identifier: $("#finishIdentifier").val(), Shape: "Rock" };
        finishGame(request);
    });
    $("#finishPaper").click(function () {
        var request = { Identifier: $("#finishIdentifier").val(), Shape: "Paper" };
        finishGame(request);
    });
    $("#finishScissors").click(function () {
        var request = { Identifier: $("#finishIdentifier").val(), Shape: "Scissors" };
        finishGame(request);
    });
    $("#checkGame").click(function () {
        var request = { Identifier: $("#checkIdentifier").val() };
        checkGame(request);
    });
}

function beginGame(request) {
    post("Begin", request, function (data) {
        $("#beginIdentifier").text("Identifier: " + data.Identifier);
    });
}

function finishGame(request) {
    post("Finish", request, function (data) {
        $("#finishOutcome").text("Outcome: " + data.Outcome);
    });
}

function checkGame(request) {
    post("Check", request, function (data) {
        $("#checkOutcome").text("Outcome: " + data.Outcome);
    });
}

function post(path, request, callback) {
    var url = "https://YOUR_API_URL_GOES_HERE/" + path;
    var requestString = JSON.stringify(request);
    $.ajax({
        url: url,
        type: "POST",
        contentType: "application/json",
        data: requestString,
        dataType: "json",
        success: callback
    });    
}
