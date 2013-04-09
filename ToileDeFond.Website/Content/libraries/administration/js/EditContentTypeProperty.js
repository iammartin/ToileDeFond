(function ($) {

    $(document).ready(function () {
        var $editContentTypePropertyForm = $('#EditContentTypePropertyForm');
       
        if ($editContentTypePropertyForm) {
            var $propertyEditor = $('#PropertyEditor');
            var $submitButton = $('#SubmitButton');

            $propertyEditor.on("change", function () {
                var route = $propertyEditor.val();
                var routes = route.split(";");
                
                if (route) {
                    $.post(routes[0], $editContentTypePropertyForm.serialize(), function (data) {
                        $('#form-container').html(data);
                        $editContentTypePropertyForm.attr('action', routes[1]);
                        $submitButton.removeAttr('disabled');
                    });
                } else {
                    /*todo: passer contenttypefullname et name - modifier l'Action*/
                    $.get('/admin/edit-content-type-property', function (data) {
                        $('#form-container').html(data);
                        $editContentTypePropertyForm.attr('action', '/admin/edit-content-type-property');
                        $submitButton.attr('disabled', 'disabled');
                    });
                }
            });
        }
    });

})(jQuery);