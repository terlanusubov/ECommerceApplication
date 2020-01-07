function ErrorModel(key, text) {
    return { errorKey: key, errorText: text }
}
function Validation(formId,photo_array,photo_input) {
    this.form_id = formId
    this.photos = photo_array;
    this.photo_input = photo_input;

    this.TryValidateForm = function () {
        var errors = [];
        var text_inputs = document.querySelectorAll(this.form_id + " " + "input[type='text']");
        var selects = document.querySelectorAll(this.form_id + " " + "select");
        for (var input of text_inputs) {
            if (input.getAttribute("data-element") == "true") {

                if (input.getAttribute("data-nullable") != "true") {
                    if (input.value == "" || input.value == null || input.value == undefined) {
                        errors.push(new ErrorModel(input.getAttribute("data-error"), "Input can not be empty"))
                    }
                }

                if (input.getAttribute("data-type") == "number") {

                    if (!isNaN(parseInt(input.value)))
                    {
                        var max = parseInt(input.getAttribute("data-max"));
                        if (parseInt(input.value) > max) {
                            errors.push(new ErrorModel(input.getAttribute("data-error"), "Data length can not be higher than " + parseInt(input.getAttribute("data-max"))))
                        }

                        var min = parseInt(input.getAttribute("data-min"));
                        if (parseInt(input.value) < min) {
                            errors.push(new ErrorModel(input.getAttribute("data-error"), "Data length can not be lower than " + parseInt(input.getAttribute("data-min"))))
                        }
                    }
                    else {

                        if (input.value != "") {
                            errors.push(new ErrorModel(input.getAttribute("data-error"), "Please write correct number"));
                        }
                    }

                }
            }
        }


        for (var select of selects) {
            if (select.getAttribute("data-element") == "true") {
                if (select.value == null || select.value == undefined || select.value == "") {
                    errors.push(new ErrorModel(select.getAttribute("data-error"), "Input can not be empty"))
                }
            }
        }


        if (this.photo_input!= null && this.photos.length == 0) {
            errors.push(new ErrorModel(this.photo_input.getAttribute("data-error"), "Add at least 1 image"));
        }
        return errors;
    }
}

