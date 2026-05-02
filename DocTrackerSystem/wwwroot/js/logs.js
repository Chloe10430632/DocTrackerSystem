$(document).ready(function () {
    fetchReadingLogs("");
    
    $("#searchBtn").on("click", function () {
        const keyword = $("#searchInput").val();
        fetchReadingLogs(keyword);
    });
});

function fetchReadingLogs(keyword) {
    $.ajax({
        url: 'https://localhost:7293/api/readinglog/search',
        type: 'GET',
        xhrFields: {
            withCredentials: true
        },
        data: { keyword: keyword },
        beforeSend: function () {
            $("#logTableBody").find("tr:not(#loadingRow)").remove();
            $("#loadingRow").show();
        },
        success: function (response) {
            $("#loadingRow").hide();
            renderTable(response);
        },
        error: function (xhr) {
            $("#loadingRow").hide();
            console.error("讀取資料失敗", xhr);
            $("#logTableBody").append('<tr><td colspan="7" class="text-center">讀取失敗，請稍後再試</td></tr>');
        }
    });
}

function renderTable(data) {
    const $tbody = $("#logTableBody");
    $tbody.empty(); 

    if (data.length === 0) {
        $tbody.append('<tr><td colspan="7" class="text-center">查無紀錄</td></tr>');
        return;
    }

    data.forEach(item => {
        const startTime = new Date(item.startTime).toLocaleString();
        const endTime = new Date(item.endTime).toLocaleString();
        const createdTime = new Date(item.createdTime).toLocaleString();

        const row = `
            <tr>
                <td><span class="text-muted">${item.logId}</span></td>
                <td>${item.userName || '匿名'}</td>
                <td>${item.docTitle}</td>
                <td>${startTime}</td>
                <td>${endTime}</td>
                <td>${createdTime}</td>
                <td>${item.clientIP}</td>
            </tr>
        `;
        $tbody.append(row);
    });
}