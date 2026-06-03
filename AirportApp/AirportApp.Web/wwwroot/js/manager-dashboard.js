document.getElementById('openAddModal')?.addEventListener('click', () => {
    document.getElementById('addModal').classList.add('open');
});
document.getElementById('cancelAdd')?.addEventListener('click', () => {
    document.getElementById('addModal').classList.remove('open');
});

document.querySelectorAll('[title="Delete flight"]').forEach(btn => {
    btn.addEventListener('click', e => {
        e.preventDefault();
        const flightId = btn.getAttribute('asp-route-flightid') || btn.closest('tr')?.querySelector('.td-id')?.textContent?.trim();
        const flightNum = btn.closest('tr')?.querySelector('.td-fnum')?.textContent?.trim();
        document.getElementById('deleteFlightId').value = flightId;
        document.getElementById('deleteConfirmText').textContent = `Are you sure you want to delete flight ${flightNum}?`;
        document.getElementById('deleteModal').classList.add('open');
    });
});
document.getElementById('cancelDelete')?.addEventListener('click', () => {
    document.getElementById('deleteModal').classList.remove('open');
});

const recurrentCheck = document.getElementById('recurrentCheck');
const recurrentSection = document.getElementById('recurrent-section');
const singleDateWrap = document.getElementById('singleDateWrap');
const customDaysWrap = document.getElementById('custom-days-wrap');

function toggleRecurrent() {
    if (recurrentCheck?.checked) {
        recurrentSection.style.display = 'block';
        singleDateWrap.style.display = 'none';
    } else {
        recurrentSection.style.display = 'none';
        singleDateWrap.style.display = 'block';
    }
}
recurrentCheck?.addEventListener('change', toggleRecurrent);
toggleRecurrent();

document.querySelector('[name="AddFlightForm.RecurrenceType"]')?.addEventListener('change', function () {
    if (customDaysWrap) customDaysWrap.style.display = this.value === 'Custom' ? 'block' : 'none';
});

document.getElementById('searchInput')?.addEventListener('keydown', e => {
    if (e.key === 'Enter') document.getElementById('searchForm').submit();
});
