function getImageScale(imageId) {
    var img = document.getElementById(imageId);
    var scale = img.width / img.naturalWidth;
    return scale;
}

function submitUploadedImage() 
{
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

function setAlertField(messageText, alertType)
{
    var alertField = document.getElementById('feedback');

    alertField.innerHTML = messageText;
    alertField.className = "alert alert-" + alertType;
    alertField.style.visibility = "visible";
}

function hideAlertField()
{
    var alertField = document.getElementById('feedback');
    alertField.style.visibility = "hidden";
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

function clearFaceRectangles()
{
    var wrapper = document.getElementById('image-wrapper'); 
    var div = wrapper.getElementsByTagName('div');
    while (div.length) {
        wrapper.removeChild(div[0]);
        var div = wrapper.getElementsByTagName('div');
    }
}

function loadProfileImageFromFile(evt) {
    var tgt = evt.target || window.event.srcElement,
        files = tgt.files;


    var uriBase =
        "/api/profile/image/face/detect";

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
                    if (data.length == 1) {
                        submitUploadedImage()
                        rect = data[0].faceRectangle; 
                        paintFaceRectangle(rect.top, rect.left, rect.width, rect.height, "#2cfa02");
                        setAlertField("Image Uploaded. Hit Save to confirm", "success");
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

    // Not supported
    else {
        Console.log("FileReader not supported")
        // fallback -- perhaps submit the input to an iframe and temporarily store
        // them on the server until the user's session ends.
    }
}