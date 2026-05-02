let startTime = new Date();
$(document).ready(function () {
 
    updateUI(1);

    $('#btn-next').on('click', function () {
        let endTime = new Date();
        let currentId = parseInt($(this).data('next-id'));
        let nextId = currentId + 1;
        


        // 重置 StartTime 為現在，準備計時下一份
        startTime = new Date();


        if (nextId > 6) {
            alert("已經是最後一份文件了！");
            return;
        }

        saveReadingLog(currentId, startTime.toISOString(), endTime.toISOString());

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

    function saveReadingLog(docId, startTime, endTime) {
        const data = {
            docId: docId,
            startTime: startTime,
            endTime: endTime
        };

        // 從 SessionStorage 或 Cookie 拿 Token (視你登入時存哪裡而定)
        //const token = sessionStorage.getItem("ApiToken");

        $.ajax({
            url: 'https://localhost:7293/api/readinglog/create',
            type: 'POST',
            contentType: 'application/json',
            xhrFields: {
                withCredentials: true
            },
            data: JSON.stringify(data),
            success: function (response) {
                console.log("紀錄已儲存");
            },
            error: function (xhr) {
                console.error("儲存失敗", xhr);
            }
        });
    }

    
});
