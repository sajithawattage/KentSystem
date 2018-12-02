
function AutoCompleteSelectHandler(event, ui)
{
    var selectedObj = ui.item.value;
    alert(selectedObj);
    
}


function allowOnlyNumber(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode

      if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 190)
   //if ( charCode != 190)
    {
        return false;
    }

    return true;
}

function Total() {

    if ($('#ctl00_ContentPlaceHolder1_txtQty').val() != "" && $('#ctl00_ContentPlaceHolder1_txtAmout').val() != "") {
        var val = Number($('#ctl00_ContentPlaceHolder1_txtQty').val()) * Number($('#ctl00_ContentPlaceHolder1_txtAmout').val());
        $('#ctl00_ContentPlaceHolder1_txtTotal').val(val);
    }
    else { $('#ctl00_ContentPlaceHolder1_txtTotal').val(''); }
}
function Validation(elementID, Msg, lblName, div) {
    var ObjectVal = $('#' + elementID).val();
    if (ObjectVal != "") {
        $('#' + lblName).html('');
        $('#' + div).parent().removeClass('has-error');
        return true;
    }
    else {

        $('#' + div).parent().addClass('has-error');
        $('#' + lblName).html('<i class="fa fa-exclamation-triangle"></i>' + Msg);
        $('#' + div).css("display", "block");
        return false;

    }

}
