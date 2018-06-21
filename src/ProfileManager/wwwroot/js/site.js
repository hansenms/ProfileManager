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
            document.getElementById('Edit').submit();
        },
        cache: false,
        contentType: false,
        processData: false
    });
    
    return false;
}

function paintFaceRectangle(top, left, width, height) {

    var box = document.getElementById('faceRectangle');
    var img = document.getElementById('outImage');

    var scale = img.width / img.naturalWidth;

    box.style.top = Math.round(top * scale).toString() + "px";
    box.style.left = Math.round(left * scale).toString() + "px";
    box.style.width = Math.round(width * scale).toString() + "px";
    box.style.height = Math.round(height * scale).toString() + "px";
    box.style.visibility = "visible";
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
                    var rect = data[0].faceRectangle;
                    if (data.length == 1) {
                        submitUploadedImage() 
                    } else {
                        alert('Invalid image. Number of faces: ' +  data.length.toString());
                    }

                    paintFaceRectangle(rect.top, rect.left, rect.width, rect.height);
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