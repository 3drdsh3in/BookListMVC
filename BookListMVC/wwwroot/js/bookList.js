let dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {

    dataTable = $('#DT_load').DataTable({
        "ajax": {
            // Pathing For Web API Calls:
            // /Controller_Name/Action_Name
            // So this would be /Books/getall to get all Books (See the GetAll methods in BooksController)
            "url": "/Books/getall",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            // Ensure that you lowercase the attribute fields on
            // If Model field is ISBN => isbn
            { "data": "name", "width": "20%" },
            { "data": "author", "width": "20%" },
            { "data": "isbn", "width": "20%" },
            {
                // Pathing For Web API Calls:
                // /Controller_Name/Action_Name
                // So this would be /Books/Upsert to Create or Update (Upsert)
                "data": "id",
                "render": function (data) {
                    return `<div class="text-center">
                        <a href="/Books/Upsert?id=${data}" class='btn btn-success text-white' style='cursor:pointer; width:70px;'>
                            Edit
                        </a>
                        &nbsp;
                        <a class='btn btn-danger text-white' style='cursor:pointer; width:70px;'
                            onclick="deleteBook('/Books/Delete?id='+${data})">
                            Delete
                        </a>
                        </div>`;
                }, "width": "40%"
            }
        ],
        "language": {
            "emptyTable": "no data found"
        },
        "width": "100%"
    })

}

function deleteBook(url) {
    swal.fire({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover",
        icon: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        closeOnCancel: true
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}