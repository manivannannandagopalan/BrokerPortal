const modal = document.querySelector('#invite-modal');
const inviteButton = document.querySelector('#invite-button');
const closeButton = document.querySelector('.modal-close');
const inviteForm = document.querySelector('#invite-form');
const breadcrumb = document.querySelector('#breadcrumb-current');

function showModal() {
  modal.hidden = false;
  modal.querySelector('input').focus();
}
function hideModal() {
  modal.hidden = true;
}
function setView(view) {
  const label = view === 'users' ? 'User management' : view === 'access' ? 'Access policies' : view === 'integrations' ? 'Integrations' : 'Overview';
  breadcrumb.textContent = label;
  document.querySelectorAll('.nav-item').forEach((item) => item.classList.toggle('active', item.dataset.view === view));
  if (view !== 'overview') {
    window.alert(`${label} is ready for the next implementation increment.`);
  }
}

inviteButton.addEventListener('click', showModal);
closeButton.addEventListener('click', hideModal);
modal.addEventListener('click', (event) => {
  if (event.target === modal) hideModal();
});
document.addEventListener('keydown', (event) => {
  if (event.key === 'Escape' && !modal.hidden) hideModal();
});
inviteForm.addEventListener('submit', (event) => {
  event.preventDefault();
  const email = new FormData(inviteForm).get('email');
  hideModal();
  inviteForm.reset();
  window.alert(`Invitation prepared for ${email}.`);
});
document.querySelectorAll('[data-view], [data-view-link]').forEach((element) => {
  element.addEventListener('click', () => setView(element.dataset.view || element.dataset.viewLink));
});
