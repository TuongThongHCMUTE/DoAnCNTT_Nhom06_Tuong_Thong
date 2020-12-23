var productCategory = {
    init: function () {
        productCategory.registerEvents();
    },
    registerEvents: function () {
        $('.btn-active').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/ProductCategory/ChangeStatus",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Còn kinh doanh');
                    }
                    else {
                        btn.text('Ngừng kinh doanh');
                    }
                }
            });
        });
    }
}
productCategory.init();