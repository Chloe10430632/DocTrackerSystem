$(document).ready(function () {
 

    $('#btn-next').on('click', function () {
        let currentId = parseInt($(this).data('next-id'));
        let nextId = currentId + 1;

        if (nextId > 6) {
            alert("已經是最後一份文件了！");
            return;
        }

        updateUI(nextId);

        $(this).data('next-id', nextId);
    });

    function updateUI(id) {
        const apiUrl = `/api/document/${id}`;

        $.ajax({
            url: apiUrl,
            method: 'GET',
            success: function (data) {
                $('#doc-title').text(data.title);
                $('#doc-content').text(data.content);
            },
            error: function () {
                alert("讀取文件失敗，請稍後再試。");
            }
        });
    }
});
