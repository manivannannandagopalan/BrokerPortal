import { ChangeDetectionStrategy, Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AppComponent {
  view = 'overview';
  userSearch = '';
  guidelineSearch = '';
  submissionStage = 'all';
  guidelineLine = 'all';
  dctImportResult: { imported: number; updated: number; skipped: number; errors: { row: number; column?: string; message: string }[] } | null = null;
  readonly users = [
    { initials: 'JR', name: 'Jordan Rivers', email: 'jordan.rivers@northstar.com', broker: 'Northstar Financial', role: 'Broker admin', status: 'Active', tone: 'coral' },
    { initials: 'SK', name: 'Sarah Kim', email: 'sarah.kim@meridian.com', broker: 'Meridian Partners', role: 'Operations', status: 'Active', tone: 'green' },
    { initials: 'MC', name: 'Marcus Chen', email: 'marcus.chen@bluerock.com', broker: 'BlueRock Insurance', role: 'Broker admin', status: 'Pending', tone: 'blue' },
    { initials: 'EC', name: 'Elena Cruz', email: 'elena.cruz@northstar.com', broker: 'Northstar Financial', role: 'Viewer', status: 'Active', tone: 'purple' }
  ];
  readonly submissions = [
    { applicant: 'Harbor Street Cafe', line: 'General liability', broker: 'Northstar Financial', stage: 'Needs review', updated: '18 minutes ago' },
    { applicant: 'Pine & Co. Contractors', line: 'Workers compensation', broker: 'Meridian Partners', stage: 'Quote ready', updated: '42 minutes ago' },
    { applicant: 'Atlas Design Studio', line: 'Business owners policy', broker: 'BlueRock Insurance', stage: 'Needs review', updated: '2 hours ago' },
    { applicant: 'Juniper Retail Group', line: 'Commercial property', broker: 'Northstar Financial', stage: 'Bound', updated: 'Yesterday' }
  ];
  readonly guidelines = [
    { title: 'General liability: artisan contractors', line: 'General liability', summary: 'Eligible risks with annual revenue up to $5M and no high-hazard operations.', version: 'v3.2', status: 'Current', review: 'Reviewed Sep 18, 2026' },
    { title: 'Commercial property: retail occupancy', line: 'Property', summary: 'Eligible retail occupancy with protected construction and a maximum location value of $10M.', version: 'v2.8', status: 'Current', review: 'Reviewed Sep 12, 2026' },
    { title: 'Workers compensation: office risks', line: 'Workers compensation', summary: 'Standard appetite for office classes with fewer than 100 employees and no remote-site exposure.', version: 'v4.1', status: 'Review due', review: 'Review due Oct 01, 2026' }
  ];

  setView(view: string): void { this.view = view; }
  async importDctFile(event: Event): Promise<void> {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    const form = new FormData();
    form.append('file', file);
    try {
      const response = await fetch('http://127.0.0.1:5082/api/dct-useradmin/import', { method: 'POST', body: form });
      this.dctImportResult = await response.json();
    } catch {
      this.dctImportResult = { imported: 0, updated: 0, skipped: 0, errors: [{ row: 0, message: 'FastAPI is not reachable. Start the local API first.' }] };
    }
    input.value = '';
  }
  get filteredUsers() { const q = this.userSearch.toLowerCase(); return this.users.filter(user => !q || `${user.name} ${user.email} ${user.broker}`.toLowerCase().includes(q)); }
  get filteredSubmissions() { return this.submissions.filter(item => this.submissionStage === 'all' || item.stage === this.submissionStage); }
  get filteredGuidelines() { const q = this.guidelineSearch.toLowerCase(); return this.guidelines.filter(item => (!q || `${item.title} ${item.summary}`.toLowerCase().includes(q)) && (this.guidelineLine === 'all' || item.line === this.guidelineLine)); }
}