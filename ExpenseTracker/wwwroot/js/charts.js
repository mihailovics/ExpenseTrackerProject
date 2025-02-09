document.addEventListener("DOMContentLoaded", function () {
    var incomeCtx = document.getElementById("incomeChart").getContext("2d");
    var expenseCtx = document.getElementById("expenseChart").getContext("2d");

    if (incomeCtx && expenseCtx) {
        var incomeData = incomeCtx.dataset.data;
        var incomeLabels = JSON.parse(incomeCtx.datasets.labels);
       

        var expenseLabels = JSON.parse(expenseCtx.dataset.labels);
        var expenseData = JSON.parse(expenseCtx.dataset.data);


        new Chart(expenseCtx, {
            type: "bar",
            data: {
                labels: expenseLabels,
                datasets: [{
                    label: "Total Expense",
                    data: expenseData,
                    backgroundColor: "rgba(255, 99, 132, 0.5)",
                    borderColor: "rgba(255, 99, 132, 1)",
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: { y: { beginAtZero: true } }
            }
        });
        new Chart(incomeCtx, {
            type: "bar",
            data: {
                labels: incomeLabels,
                datasets: [{
                    label: "Total Income",
                    data: incomeData,
                    backgroundColor: "rgba(54, 162, 235, 0.5)",
                    borderColor: "rgba(54, 162, 235, 1)",
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }
});