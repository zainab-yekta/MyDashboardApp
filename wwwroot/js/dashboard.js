// Draw the sales chart with data rendered by the server
const chart = new Chart(document.getElementById('salesChart'), {
    type: 'line',
    data: {
        labels: salesLabels,
        datasets: [{
            label: 'Sales',
            data: salesValues,
            borderColor: '#1b6ec2',
            backgroundColor: 'rgba(27, 110, 194, 0.15)',
            fill: true,
            tension: 0.3
        }]
    },
    options: {
        plugins: { legend: { display: false } },
        scales: { y: { beginAtZero: true } }
    }
});

// Recalculate the KPI cards from the chart data
function updateKpis() {
    const values = chart.data.datasets[0].data;
    const total = values.reduce((sum, v) => sum + v, 0);
    const best = values.indexOf(Math.max(...values));

    document.getElementById('kpi-total').textContent = total.toLocaleString();
    document.getElementById('kpi-best').textContent = chart.data.labels[best];
    document.getElementById('kpi-average').textContent = Math.floor(total / values.length).toLocaleString();
}

// Show the new sale at the top of the recent list, keep 5 items
function addRecentSale(month, sales) {
    const list = document.getElementById('recent-sales');
    const item = document.createElement('li');
    item.className = 'list-group-item d-flex justify-content-between px-0';
    item.innerHTML = '<span></span><strong></strong>';
    item.children[0].textContent = month;
    item.children[1].textContent = sales.toLocaleString();
    list.prepend(item);

    while (list.children.length > 5) {
        list.lastElementChild.remove();
    }
}

// Connect to SignalR Hub, every open dashboard gets new sales
const connection = new signalR.HubConnectionBuilder()
    .withUrl('/chartHub')
    .withAutomaticReconnect()
    .build();

connection.on('SaleAdded', (month, sales) => {
    chart.data.labels.push(month);
    chart.data.datasets[0].data.push(sales);
    chart.update();
    updateKpis();
    addRecentSale(month, sales);
});

const status = document.getElementById('live-status');
connection.onreconnecting(() => { status.textContent = 'Reconnecting...'; status.className = 'badge text-bg-warning'; });
connection.onreconnected(() => { status.textContent = 'Live'; status.className = 'badge text-bg-success'; });

connection.start()
    .then(() => { status.textContent = 'Live'; status.className = 'badge text-bg-success'; })
    .catch(err => console.error(err));

// Admin form: post with the antiforgery token, the hub sends the update back
const form = document.getElementById('sale-form');
if (form) {
    form.addEventListener('submit', async (e) => {
        e.preventDefault();
        const response = await fetch(form.action, { method: 'POST', body: new FormData(form) });

        if (response.ok) {
            form.reset();
            document.getElementById('sale-error').textContent = '';
        } else {
            document.getElementById('sale-error').textContent = 'Could not add the sale. Check the values and try again.';
        }
    });
}
