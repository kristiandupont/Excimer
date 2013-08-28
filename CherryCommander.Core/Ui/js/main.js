var viewModel;
var selection = [];

var panes = ["leftPane", "rightPane"];
var ENTER_KEY = 13;

ko.bindingHandlers.enterKey = {
    init: function (element, valueAccessor, allBindingsAccessor, data) {
        var wrappedHandler, newValueAccessor;

        // wrap the handler with a check for the enter key
        wrappedHandler = function (data, event) {
            if (event.keyCode === ENTER_KEY) {
                valueAccessor().call(this, data, event);
            }
        };

        // create a valueAccessor with the options that we would want to pass to the event binding
        newValueAccessor = function () {
            return {
                keyup: wrappedHandler
            };
        };

        // call the real event binding's init function
        ko.bindingHandlers.event.init(element, newValueAccessor, allBindingsAccessor, data);
    }
};

function getFiles(pane, directory) {
    $.get("/api/GetFiles", { directory: directory }, function (files) {
        viewModel[panes[pane]](files);
        select(selection[0], 0);
    });
}

function select(pane, row) {

    if (selection.length != 0) {
        $("#" + panes[selection[0]] + " tr").eq(selection[1]).removeClass("selected");
    }

    selection = [pane, row];
    $("#" + panes[selection[0]] + " tr").eq(selection[1]).addClass("selected");
}

function getSelectedFile() {
    var pane = panes[selection[0]];
    var row = selection[1];

    return viewModel[pane]()[row];
}

$(function () {

    viewModel = {
        leftPane: ko.observableArray([]),
        rightPane: ko.observableArray([]),

        open: function (item) {
            console.log("Open!");
            var f = getSelectedFile();
            if (f.isDirectory) {
                console.log("Entering dir: " + f.filename);
                getFiles(selection[0], f.fullPath);
            } else {
                console.log("Executing: " + f.fullPath);
            }
        }
    };

    ko.applyBindings(viewModel);

    getFiles(0, "c:\\");
    getFiles(1, "Z:\\kristian_up\\Repositories\\Excimer\\CherryCommander.Core\\Ui\\js\\vendor");

    select(0, 0);

//    $(document).keydown(function (e) {
//        switch (e.which) {
//            //            case 13: 
//            //                var f = getSelectedFile(); 
//            //                if (f.isDirectory) { 
//            //                    console.log("Entering dir: " + f.filename); 
//            //                    getFiles(selection[0], f.fullPath); 
//            //                } 
//            //                else { 
//            //                    console.log("Executing: " + f.fullPath); 
//            //                } 
//            //                break; 
//            case 37: // left
//            case 39: // right

//                var newPane = 1 - selection[0];
//                var newRow = selection[1];

//                var pane = panes[newPane];
//                if (newRow >= viewModel[pane]().length)
//                    newRow = viewModel[pane]().length - 1;

//                select(newPane, newRow);
//                break;

//            case 38: // up
//                if (selection[1] > 0)
//                    select(selection[0], selection[1] - 1);
//                break;

//            case 40: // down
//                var pane = panes[selection[0]];

//                if (selection[1] < viewModel[pane]().length - 1)
//                    select(selection[0], selection[1] + 1);
//                break;

//            default: return; // exit this handler for other keys
//        }
//        e.preventDefault();
//    });

});

