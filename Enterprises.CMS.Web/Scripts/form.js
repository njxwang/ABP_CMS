(function () {
    $(function () {
        // 提交表单
        function sumbitModalForm(url) {
            $.get(url, null, function (html) {
                var pageModel = $('#pageModal');
                pageModel.html("").html(html);
                pageModel.modal("show");
                var dataForm = $('#saveForm', pageModel);
                $('#btSave').on("click", function (e) {
                    e.preventDefault();
                    abp.ajax({
                        url: dataForm.attr("action"),
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/x-www-form-urlencoded',
                        data: dataForm.formSerialize()
                    }).done(function (data) {
                        if (data.result) {
                            pageModel.modal("hide");
                            abp.notify.success("保存成功！");
                            location.reload();
                        } else {
                            $.each(data.errors, function (index, error) {
                                var container = $('span[data-valmsg-for="' + error.key + '"]');
                                container.removeClass("field-validation-valid").addClass("field-validation-error");
                                container.html(error.message);
                                $(container).parents('.control-group').addClass('error');
                                $("#" + error.key).on("change", function () {
                                    $(this).parents('.control-group').removeClass('error');
                                    container.removeClass("field-validation-error").addClass("field-validation-valid").html("");
                                });
                            });
                        }

                    }).fail(function (data) {
                        abp.notify.error("发生错误！");
                    });
                });
            });
        }

        // 增加列表行
        $("#btCreate").click(function (e) {
            var url = $(this).attr("url");
            sumbitModalForm(url);
        });

        // 编辑列表行
        $("#AjaxPagerPangel").on("click", ".editflag", function () {
            var url = $(this).attr("url");
            sumbitModalForm(url);
        });

        // 删除列表行
        $("#AjaxPagerPangel").on("click", ".deleteflag", function () {
            var url = $(this).attr("url");
            abp.message.confirm("", "确定要删除吗？",
                function (isConfirm) {
                    if (isConfirm) {
                        abp.ajax({
                            url: url,
                            type: 'get'
                        }).done(function (data) {
                            abp.notify.success("删除成功！");
                            location.reload();
                        }).fail(function (data) {
                            abp.notify.error("删除失败！");
                        });
                    }
                }
            );
        });
    });
})();