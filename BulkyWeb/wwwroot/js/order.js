var dataTable;

$(document).ready(() => loadDataTable());

function loadDataTable() {
    dataTable = $('#OrderTable').DataTable({
        "ajax": { url: "/API/Admin/Order/GetAll" },
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
                            href="/admin/order/details/?id=${data}"
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
