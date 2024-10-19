function getMoreProduct(parentId, pageSize = null, currentCount = null, type = 0) {
    $(function () {
        if (parentId == 0 || parentId == null) { parentId = $("#parentIdDefault").val(); }
        if ($("input[name=parentId]:checked").val()) { parentId = $("input[name=parentId]:checked").val(); }
        var t = $("input[name='__RequestVerificationToken']").val();
        pageSize = pageSize < +$("input[name='pageSize']").val() ? +$("input[name='pageSize']").val() : pageSize;
        var req = {
            parentId: parentId,
            currentCount: currentCount == null ? $(".product-item").length : currentCount,
            pageSize: pageSize == null ? $(".product-item").length : pageSize,
            title: $("input[name='title']").val() ?? null,
        };
        $.ajax({
            type: "POST",
            url: "/umbraco/surface/Catalog/SearchProduct",
            data: req,
            headers:
            {
                "RequestVerificationToken": t
            },
            encode: true
        }).done(function (data) {
            if (type == 0)
                $("#main-block-product-list").html(data);
            else if (type == 1)
                $("#main-block-product-list").append(data);

            if ($(".product-item").length >= +$("input[name='totalCount']").val() || data == "") {
                $("#btnGetMoreProduct").hide();
            } else {
                $("#btnGetMoreProduct").show();
            }
        });
    });
}