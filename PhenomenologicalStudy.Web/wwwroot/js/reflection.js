﻿var dataTable;

$(document).ready(function () {
  loadDataTable();
});

function loadDataTable() {
  dataTable = $('#tblData').DataTable({
    "ajax": {
      "url": "/Reflections",
      "type": "GET",
      "dataType": "json"
    },
    "columns": [
      {"data": "name", "width": "50%"},
      {"data": "state", "width": "20%"},
      {
        "data": "id",
        "render": function (data) {
          return `<div class="text-center">
                    <a href="/Reflections/Details/${data}" class="btn btn-success text-white" style="cursor:pointer;"> <i class="far fa-edit"></i></a>
                    &nbsp;
                    <a onClick=Delete("/Reflections/Delete/${data}") class="btn btn-danger text-white" style="cursor:pointer;"> <i class="far fa-trash-alt"></i></a>
                  </div>`;
        },
        "width": "30%"
      },
    ]
  });
}

function Delete(url) {
  // Configure a "sweetalert" (from JS package included in _Layout as script) on deleting to confirm.
  swal({
    title: "Are you sure you want to Delete?",
    text: "You will not be able to restore the data!",
    icon: "warning",
    buttons: true,
    dangerMode: true
  }).then((willDelete) => {
    if (willDelete) {
      $.ajax({
        type: "DELETE",
        url: url,
        success: function (data) {
          if (data.success) {
            toastr.success(data.message);
            dataTable.ajax.reload();
          } else {
            toastr.error(data.message);
          }
        }
      })
    }
  });
}