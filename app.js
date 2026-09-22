const modal = document.querySelector('#invite-modal');
const inviteButton = document.querySelector('#invite-button');
const closeButton = document.querySelector('.modal-close');
const inviteForm = document.querySelector('#invite-form');
const breadcrumb = document.querySelector('#breadcrumb-current');
const overviewView = document.querySelector('#overview-view');
const usersView = document.querySelector('#users-view');
const accessView = document.querySelector('#access-view');
const integrationsView = document.querySelector('#integrations-view');
const usersInviteButton = document.querySelector('#users-invite-button');
const userRows = document.querySelector('#user-rows');
const userSearch = document.querySelector('#user-search');
const userStatus = document.querySelector('#user-status');
const users = [
  { initials: 'JR', name: 'Jordan Rivers', email: 'jordan.rivers@northstar.com', broker: 'Northstar Financial', role: 'Broker admin', signIn: 'Today, 9:42 AM', status: 'active', tone: 'coral' },
  { initials: 'SK', name: 'Sarah Kim', email: 'sarah.kim@meridian.com', broker: 'Meridian Partners', role: 'Operations', signIn: 'Today, 8:16 AM', status: 'active', tone: 'green' },
  { initials: 'MC', name: 'Marcus Chen', email: 'marcus.chen@bluerock.com', broker: 'BlueRock Insurance', role: 'Broker admin', signIn: 'Yesterday', status: 'pending', tone: 'blue' },
  { initials: 'EC', name: 'Elena Cruz', email: 'elena.cruz@northstar.com', broker: 'Northstar Financial', role: 'Viewer', signIn: 'Yesterday', status: 'active', tone: 'purple' },
  { initials: 'DW', name: 'Daniel Wu', email: 'daniel.wu@meridian.com', broker: 'Meridian Partners', role: 'Operations', signIn: 'Sep 19, 2026', status: 'active', tone: 'orange' },
  { initials: 'PA', name: 'Priya Anand', email: 'priya.anand@bluerock.com', broker: 'BlueRock Insurance', role: 'Viewer', signIn: 'Invitation sent', status: 'pending', tone: 'teal' }
];

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
  overviewView.hidden = view !== 'overview';
  usersView.hidden = view !== 'users';
  accessView.hidden = view !== 'access';
  integrationsView.hidden = view !== 'integrations';
  if (view === 'users') renderUsers();
}

function renderUsers() {
  const query = userSearch.value.trim().toLowerCase();
  const status = userStatus.value;
  const filtered = users.filter((user) => {
    const matchesQuery = !query || `${user.name} ${user.email} ${user.broker}`.toLowerCase().includes(query);
    return matchesQuery && (status === 'all' || user.status === status);
  });
  document.querySelector('#user-count').textContent = filtered.length;
  userRows.innerHTML = filtered.map((user) => `<tr><td><div class="broker-name"><span class="review-avatar ${user.tone}">${user.initials}</span><span><strong>${user.name}</strong><small>${user.email}</small></span></div></td><td>${user.broker}</td><td><span class="role-label">${user.role}</span></td><td>${user.signIn}</td><td><span class="status-pill ${user.status === 'active' ? 'live' : 'pending'}">${user.status === 'active' ? 'Active' : 'Pending'}</span></td><td><button class="row-menu" aria-label="${user.name} actions">&#8942;</button></td></tr>`).join('') || '<tr><td colspan="6" class="empty-state">No users match these filters.</td></tr>';
}

inviteButton.addEventListener('click', showModal);
usersInviteButton.addEventListener('click', showModal);
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
userSearch.addEventListener('input', renderUsers);
userStatus.addEventListener('change', renderUsers);
document.querySelector('#policy-refresh').addEventListener('click', (event) => {
  event.currentTarget.textContent = 'Policies are current';
});
document.querySelector('#integration-refresh').addEventListener('click', (event) => {
  event.currentTarget.textContent = 'Health checks complete';
});
