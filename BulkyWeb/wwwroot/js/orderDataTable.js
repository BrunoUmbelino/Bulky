var dataTable;

$(document).ready(() => {
    var urlParams = new URLSearchParams(window.location.search);
    var filterStatus = urlParams.get("filterStatus");
    loadDataTable(filterStatus);
});

function loadDataTable(filterStatus) {
    dataTable = $('#OrderTable').DataTable({
        "ajax": { url: `/API/Admin/Order/GetAll?filterStatus=${filterStatus}` },
        "columns": [
            { data: "id", "width": "20%" },
            { data: "name", "width": "15%" },
            { data: "phoneNumber", "width": "20%" },
            { data: "applicationUser.email", "width": "10%" },
            { data: "orderTotal", width: "15%" },
            {
                data: "id",
                "width": "10%",
                "render": function (data) {
                    return `
                    <div class="w-75 btn-group" role="group">
                        <a class="btn btn-outline-warning mx-1"
                            href="/admin/order/details/?orderId=${data}"
                            <i class="bi bi-pencil-square"></i> Details
                        </a>
                    </div>
                    `
                }
            }
        ],
        "responsive": true
    });
}
