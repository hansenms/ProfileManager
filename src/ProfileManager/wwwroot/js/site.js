function getImageScale(imageId) {
    var img = document.getElementById(imageId);
    var scale = img.width / img.naturalWidth;
    return scale;
}

function submitUploadedImage() {
    var imageFileInput = document.getElementById('file');
    var formData = new FormData();
    formData.append("file", imageFileInput.files[0]);

    var uri = "/api/profile/image/" + document.getElementById('Id').value.toString();

    $.ajax({
        url: uri,
        type: 'POST',
        data: formData,
        mimeType: "multipart/form-data",
        success: function (data) {
            //
        },
        cache: false,
        contentType: false,
        processData: false
    });

    return false;
}

function setAlertField(messageText, alertType) {
    var alertField = document.getElementById('feedback');

    alertField.innerHTML = messageText;
    alertField.className = "alert alert-" + alertType;
    alertField.style.visibility = "visible";
}

function hideAlertField() {
    var alertField = document.getElementById('feedback');
    alertField.style.visibility = "hidden";
}

function disableSubmitButton() {
    document.getElementById('edit-submit').disabled = true;
}

function enableSubmitButton() {
    document.getElementById('edit-submit').disabled = false;
}

function paintFaceRectangle(top, left, width, height, color) {

    var wrapper = document.getElementById('image-wrapper');
    var box = document.createElement('div');
    box.className = "box";
    box.style.border = "2px solid " + color;

    wrapper.appendChild(box);

    var img = document.getElementById('outImage');

    var scale = img.width / img.naturalWidth;

    box.style.top = Math.round(top * scale).toString() + "px";
    box.style.left = Math.round(left * scale).toString() + "px";
    box.style.width = Math.round(width * scale).toString() + "px";
    box.style.height = Math.round(height * scale).toString() + "px";
    box.style.visibility = "visible";
}

function clearFaceRectangles() {
    var wrapper = document.getElementById('image-wrapper');
    var div = wrapper.getElementsByTagName('div');
    while (div.length) {
        wrapper.removeChild(div[0]);
        var div = wrapper.getElementsByTagName('div');
    }
}

//For reasons unknow, it is not possible to send the blob with $.ajax,
//TODO: Look into what the issue might be and switch.
function faceDetectAnalysis(imgData, doneFunction) {
    var uriBase = "/api/profile/image/face/detect";

    var params = {
        "returnFaceId": "true"
    };

    var oReq = new XMLHttpRequest();
    oReq.open("POST", uriBase + "?" + $.param(params));
    oReq.setRequestHeader("Content-Type", "application/octet-stream", true);
    oReq.send(imgData);

    oReq.onload = function (oEvent) {
        if (oReq.status == 200) {
            doneFunction(JSON.parse(oReq.response));
        } else {
            doneFunction("[]");
            console.log("Error " + oReq.status + " occurred while attempting to send image.");
        }
    };
}

function loadNewProfileImageFromFile(evt) {
    var tgt = evt.target || window.event.srcElement,
        files = tgt.files;

    var uriBase = "/api/profile/image/face/detect";

    // Request parameters.
    var params = {
        "returnFaceId": "true"
    };

    clearFaceRectangles();
    disableSubmitButton();
    setAlertField("Analyzing image...", "light");
    // FileReader support
    if (FileReader && files && files.length) {
        var fr = new FileReader();
        fr.onload = function () {

            $.ajax({
                url: uriBase + "?" + $.param(params),

                // Request headers.
                beforeSend: function (xhrObj) {
                    xhrObj.setRequestHeader("Content-Type", "application/octet-stream");
                },
                type: "POST",
                processData: false,

                // Request body.
                data: files[0],
            })
                .done(function (data) {
                    var rect;
                    if (data.length == 1) {
                        submitUploadedImage()
                        rect = data[0].faceRectangle;
                        paintFaceRectangle(rect.top, rect.left, rect.width, rect.height, "#2cfa02");
                        setAlertField("Image Uploaded. Hit Save to confirm", "success");
                        enableSubmitButton();
                    } else {
                        var f;
                        for (f = 0; f < data.length; f++) {
                            rect = data[f].faceRectangle;
                            paintFaceRectangle(rect.top, rect.left, rect.width, rect.height, "#fa0202");
                        }
                        setAlertField("Invalid number of faces in image: " + data.length.toString(), "danger");
                        //alert('Invalid image. Number of faces: ' +  data.length.toString());
                    }

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    // Display error message.
                    var errorString = (errorThrown === "") ?
                        "Error. " : errorThrown + " (" + jqXHR.status + "): ";
                    errorString += (jqXHR.responseText === "") ?
                        "" : (jQuery.parseJSON(jqXHR.responseText).message) ?
                            jQuery.parseJSON(jqXHR.responseText).message :
                            jQuery.parseJSON(jqXHR.responseText).error.message;
                    alert(errorString);
                });

            document.getElementById('outImage').src = fr.result;
            document.getElementById('Photo').value = document.getElementById('Id').value + '/' + files[0].name;
        }
        fr.readAsDataURL(files[0]);
    }
}


function loadVerificationProfileImageFromFile(evt) {
    var tgt = evt.target || window.event.srcElement,
        files = tgt.files;


    var uriBase = "/api/profile/image/face/detect";

    // Request parameters.
    var params = {
        "returnFaceId": "true"
    };

    clearFaceRectangles();
    setAlertField("Analyzing image...", "light");
    // FileReader support
    if (FileReader && files && files.length) {
        var fr = new FileReader();
        fr.onload = function () {

            $.ajax({
                url: uriBase + "?" + $.param(params),

                // Request headers.
                beforeSend: function (xhrObj) {
                    xhrObj.setRequestHeader("Content-Type", "application/octet-stream");
                },
                type: "POST",
                processData: false,

                // Request body.
                data: files[0],
            })
                .done(function (data) {
                    var rect;
                    if (data.length > 0) {
                        verifyFaces(data);
                    } else {
                        setAlertField("No faces detected in validation image.", "danger");
                    }

                })
                .fail(function (jqXHR, textStatus, errorThrown) {
                    // Display error message.
                    var errorString = (errorThrown === "") ?
                        "Error. " : errorThrown + " (" + jqXHR.status + "): ";
                    errorString += (jqXHR.responseText === "") ?
                        "" : (jQuery.parseJSON(jqXHR.responseText).message) ?
                            jQuery.parseJSON(jqXHR.responseText).message :
                            jQuery.parseJSON(jqXHR.responseText).error.message;
                    alert(errorString);
                });

            document.getElementById('outImage').src = fr.result;
        }
        fr.readAsDataURL(files[0]);
    }
}

function verifyFaces(testFaces) {
    var img = document.getElementById('profile-photo');

    $.ajax({
        url: img.src,
        xhr: function () {// Seems like the only way to get access to the xhr object
            var xhr = new XMLHttpRequest();
            xhr.responseType = 'blob'
            return xhr;
        },
        type: 'GET',
        cache: false,
    })
        .done(function (imgData) {
            faceDetectAnalysis(
                imgData,
                function (data) {
                    //If we have some faces in the original image
                    if (data.length > 0) {
                        var fref = data[0].faceId;
                        var facesChecked = 0;
                        var matchesFound = 0;
                        for (let f = 0; f < testFaces.length; f++) {
                            let compObject = { faceId1: fref, faceId2: testFaces[f].faceId };
                            let rect = testFaces[f].faceRectangle;
                            $.ajax({
                                url: '/api/profile/image/face/verify',
                                beforeSend: function (xhrObj) {
                                    xhrObj.setRequestHeader("Content-Type", "application/json");
                                },
                                data: JSON.stringify(compObject),
                                type: 'POST',
                                success: function (data) {
                                    //The incrementing of valus here is to ensure that all the
                                    //Asyncronous HTTP requests complete before we update the status.
                                    if (data.isIdentical) {
                                        matchesFound++;
                                        paintFaceRectangle(rect.top, rect.left, rect.width, rect.height, "#2cfa02");
                                    } else {
                                        paintFaceRectangle(rect.top, rect.left, rect.width, rect.height, "#fa0202");
                                    }
                                    facesChecked++;
                                    if (facesChecked >= testFaces.length) {
                                        if (matchesFound > 0) {
                                            setAlertField("Matches found: " + matchesFound, "success");
                                        } else {
                                            setAlertField("No matches found", "danger");
                                        }
                                    }
                                },
                            })
                        }
                    } else {
                        setAlertField("No faces in original image", "danger");
                    }
                });
        });
}
