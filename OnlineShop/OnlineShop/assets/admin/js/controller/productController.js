var product = {
    init: function () {
        product.registerEvents();
    },
    registerEvents: function () {
        $('#status').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Product/ChangeStatus",
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

        $('#show-on-home').off('click').on('click', function (e) {
            e.preventDefault();
            var btn = $(this);
            var id = btn.data('id');
            $.ajax({
                url: "/Admin/Product/ChangeShowOnHome",
                data: { id: id },
                dataType: "json",
                type: "POST",
                success: function (response) {
                    console.log(response);
                    if (response.status == true) {
                        btn.text('Hiển thị');
                    }
                    else {
                        btn.text('Ẩn');
                    }
                }
            });
        });
    }
}
product.init();