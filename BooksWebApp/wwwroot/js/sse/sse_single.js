if (typeof (EventSource) !== "undefined") {
    var source = new EventSource("/Sse/Process");

    // Begin connect ! Processing started !
    source.addEventListener("open", function (event) {
        console.log("SSE: Processing started...");
    }, false);

    // When error ! Close connection !
    source.addEventListener("error", function (event) {
        if (event.eventPhase == EventSource.CLOSED) {
            console.log("SSE: Connection Closed!");
            //source.close();  Ko chạy lại khi đóng !!
        }
    }, false);

    // Listen message response from server !
    source.addEventListener("message", function (event) {
        //console.log(event);
        var data = JSON.parse(event.data);
        console.log(data);
    }, false);


} else {
    document.getElementById("resultAlert").innerHTML = "Sorry, your browser does not support server-sent events...";
}