function getMoreNews(parentId, pageSize) {
    $(function () {
        var t = $("input[name='__RequestVerificationToken']").val();
        pageSize = pageSize < +$("input[name='pageSize']").val() ? +$("input[name='pageSize']").val() : pageSize;
        var req = {
            parentId: parentId,
            currentCount: $(".newsItem").length,
            pageSize: pageSize,
        };
        $.ajax({
            type: "POST",
            url: "/umbraco/surface/News/SearchNews",
            data: req,
            headers:
            {
                "RequestVerificationToken": t
            },
            encode: true
        }).done(function (data) {
            $("#main-block-news-list").append(data);
            if ($(".newsItem").length >= +$("input[name='totalCount']").val() || data == "") {
                $("#btnGetMoreNews").hide();
            } else {
                $("#btnGetMoreNews").show();
            }
        });
    });
}