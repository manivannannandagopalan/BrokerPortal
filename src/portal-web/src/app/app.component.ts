import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  view = 'overview';
  userSearch = '';
  guidelineSearch = '';
  submissionStage = 'all';
  guidelineLine = 'all';
  dctImportResult: { imported: number; updated: number; skipped: number; errors: { row: number; column?: string; message: string }[] } | null = null;
  dctRows: { id: number; inttype: number; name: string; contact?: string; city?: string; state?: string; reference?: string }[] = [];
  users = [
    { initials: 'JR', name: 'Jordan Rivers', email: 'jordan.rivers@northstar.com', broker: 'Northstar Financial', role: 'Broker admin', status: 'Active', tone: 'coral' },
    { initials: 'SK', name: 'Sarah Kim', email: 'sarah.kim@meridian.com', broker: 'Meridian Partners', role: 'Operations', status: 'Active', tone: 'green' },
    { initials: 'MC', name: 'Marcus Chen', email: 'marcus.chen@bluerock.com', broker: 'BlueRock Insurance', role: 'Broker admin', status: 'Pending', tone: 'blue' },
    { initials: 'EC', name: 'Elena Cruz', email: 'elena.cruz@northstar.com', broker: 'Northstar Financial', role: 'Viewer', status: 'Active', tone: 'purple' }
  ];
  submissions = [
    { applicant: 'Harbor Street Cafe', line: 'General liability', broker: 'Northstar Financial', stage: 'Needs review', updated: '18 minutes ago' },
    { applicant: 'Pine & Co. Contractors', line: 'Workers compensation', broker: 'Meridian Partners', stage: 'Quote ready', updated: '42 minutes ago' },
    { applicant: 'Atlas Design Studio', line: 'Business owners policy', broker: 'BlueRock Insurance', stage: 'Needs review', updated: '2 hours ago' },
    { applicant: 'Juniper Retail Group', line: 'Commercial property', broker: 'Northstar Financial', stage: 'Bound', updated: 'Yesterday' }
  ];
  guidelines = [
    { title: 'General liability: artisan contractors', line: 'General liability', summary: 'Eligible risks with annual revenue up to $5M and no high-hazard operations.', version: 'v3.2', status: 'Current', review: 'Reviewed Sep 18, 2026' },
    { title: 'Commercial property: retail occupancy', line: 'Property', summary: 'Eligible retail occupancy with protected construction and a maximum location value of $10M.', version: 'v2.8', status: 'Current', review: 'Reviewed Sep 12, 2026' },
    { title: 'Workers compensation: office risks', line: 'Workers compensation', summary: 'Standard appetite for office classes with fewer than 100 employees and no remote-site exposure.', version: 'v4.1', status: 'Review due', review: 'Review due Oct 01, 2026' }
  ];
  policies: { name: string; permissions: string[]; status: string }[] = [];
  integrations: { name: string; status: string; latency_ms?: number }[] = [];
  auditEvents: { event_type: string; actor: string; broker_scope?: string; outcome: string }[] = [];

  setView(view: string): void {
    this.view = view;
    if (view === 'users') void this.loadUsers();
    if (view === 'commercial') void this.loadSubmissions();
    if (view === 'guidelines') void this.loadGuidelines();
    if (view === 'access') void this.loadPolicies();
    if (view === 'integrations') void this.loadIntegrations();
    if (view === 'audit') void this.loadAuditEvents();
    if (view === 'dct-useradmin') void this.loadDctRows();
  }
  async loadUsers(): Promise<void> {
    try {
      const response = await fetch('http://127.0.0.1:5082/api/users');
      if (response.ok) this.users = (await response.json()).map((user: any) => ({ ...user, initials: user.name.split(' ').map((part: string) => part[0]).join('').slice(0, 2), broker: user.broker_id, status: user.status[0].toUpperCase() + user.status.slice(1), tone: 'blue' }));
    } catch { /* API status is reflected by the empty state. */ }
  }
  async loadSubmissions(): Promise<void> {
    try {
      const response = await fetch('http://127.0.0.1:5082/api/commercial/submissions');
      if (response.ok) this.submissions = (await response.json()).map((item: any) => ({ applicant: item.applicant, line: item.line_of_business, broker: item.broker_id, stage: item.stage, updated: item.updated_at }));
    } catch { /* API status is reflected by the seed fallback. */ }
  }
  async loadGuidelines(): Promise<void> {
    try {
      const response = await fetch('http://127.0.0.1:5082/api/underwriting/guidelines');
      if (response.ok) this.guidelines = (await response.json()).map((item: any) => ({ ...item, review: item.effective_date }));
    } catch { /* API status is reflected by the seed fallback. */ }
  }
  async loadPolicies(): Promise<void> { try { const response = await fetch('http://127.0.0.1:5082/api/policies'); if (response.ok) this.policies = await response.json(); } catch { this.policies = []; } }
  async loadIntegrations(): Promise<void> { try { const response = await fetch('http://127.0.0.1:5082/api/integrations/health'); if (response.ok) this.integrations = await response.json(); } catch { this.integrations = []; } }
  async loadAuditEvents(): Promise<void> { try { const response = await fetch('http://127.0.0.1:5082/api/audit-events'); if (response.ok) this.auditEvents = await response.json(); } catch { this.auditEvents = []; } }
  async loadDctRows(): Promise<void> {
    try {
      const response = await fetch('http://127.0.0.1:5082/api/dct-useradmin');
      if (response.ok) this.dctRows = await response.json();
    } catch {
      this.dctRows = [];
    }
  }
  async importDctFile(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    const form = new FormData();
    form.append('file', file);
    try {
      const response = await fetch('http://127.0.0.1:5082/api/dct-useradmin/import', { method: 'POST', body: form });
      const result = await response.json();
      this.dctImportResult = response.ok ? result : { imported: 0, updated: 0, skipped: 0, errors: [{ row: 0, message: result.detail || `Import failed with HTTP ${response.status}` }] };
      if (response.ok) await this.loadDctRows();
    } catch {
      this.dctImportResult = { imported: 0, updated: 0, skipped: 0, errors: [{ row: 0, message: 'FastAPI is not reachable. Start the local API first.' }] };
    }
    input.value = '';
  }
  get filteredUsers() { const q = this.userSearch.toLowerCase(); return this.users.filter(user => !q || `${user.name} ${user.email} ${user.broker}`.toLowerCase().includes(q)); }
  get filteredSubmissions() { return this.submissions.filter(item => this.submissionStage === 'all' || item.stage === this.submissionStage); }
  get filteredGuidelines() { const q = this.guidelineSearch.toLowerCase(); return this.guidelines.filter(item => (!q || `${item.title} ${item.summary}`.toLowerCase().includes(q)) && (this.guidelineLine === 'all' || item.line === this.guidelineLine)); }
}