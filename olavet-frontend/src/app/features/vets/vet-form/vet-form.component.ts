import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { VetService } from '../../../core/services/vet.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-vet-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="form-page">
      <a routerLink="/vets" class="back-link">← Back</a>
      <h1>{{ isEdit ? 'Edit Veterinarian' : 'Add Veterinarian' }}</h1>
      <form [formGroup]="form" (ngSubmit)="submit()" class="form-card">
        <div class="form-row">
          <div class="form-group"><label>Vet Name *</label><input formControlName="vetName" /></div>
          <div class="form-group"><label>Specialization *</label><input formControlName="specialization" /></div>
        </div>
        <div class="form-row">
          <div class="form-group"><label>Email *</label><input formControlName="email" type="email" /></div>
          <div class="form-group"><label>Phone *</label><input formControlName="contactNumber" /></div>
        </div>
        <div class="form-row">
          <div class="form-group"><label>Fee *</label><input formControlName="fee" type="number" min="0" step="0.01" /></div>
          <div class="form-group"><label>Experience (years)</label><input formControlName="yearsOfExperience" type="number" min="0" /></div>
        </div>
        <div class="form-group"><label>Address</label><input formControlName="address" /></div>
        <div class="form-actions">
          <a routerLink="/vets" class="btn btn-cancel">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading">
            @if (loading) { <span class="spinner"></span> }
            {{ isEdit ? 'Update' : 'Create' }}
          </button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-page { max-width: 640px; margin: 0 auto; }
    .back-link { display: inline-block; margin-bottom: 0.75rem; color: #667eea; text-decoration: none; &:hover { text-decoration: underline; } }
    h1 { margin: 0 0 1.5rem; font-size: 1.5rem; }
    .form-card { background: #fff; border-radius: 12px; padding: 2rem; box-shadow: 0 2px 8px rgba(0,0,0,0.04); }
    .form-group { margin-bottom: 1.25rem; display: flex; flex-direction: column;
      label { font-weight: 500; margin-bottom: 0.35rem; font-size: 0.9rem; color: #333; }
      input { padding: 0.6rem 0.9rem; border: 1px solid #e2e8f0; border-radius: 8px; font-size: 0.9rem;
        &:focus { outline: none; border-color: #667eea; box-shadow: 0 0 0 3px rgba(102,126,234,0.1); } } }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .form-actions { display: flex; justify-content: flex-end; gap: 0.75rem; margin-top: 1rem; }
    .btn { padding: 0.55rem 1.25rem; border: none; border-radius: 8px; font-weight: 600; font-size: 0.9rem; cursor: pointer; text-decoration: none; display: inline-flex; align-items: center; gap: 0.4rem;
      &-primary { background: #667eea; color: #fff; &:hover { background: #5a6fd6; } &:disabled { opacity: 0.6; } }
      &-cancel { background: #edf2f7; color: #555; &:hover { background: #e2e8f0; } } }
    .spinner { width: 16px; height: 16px; border: 2px solid rgba(255,255,255,0.3); border-top-color: #fff; border-radius: 50%; animation: spin 0.6s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }
  `],
})
export class VetFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private vetService = inject(VetService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);
  form!: FormGroup;
  isEdit = false;
  vetId: number | null = null;
  loading = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      vetName: ['', [Validators.required, Validators.maxLength(200)]],
      specialization: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: ['', Validators.required],
      fee: [null, [Validators.required, Validators.min(0)]],
      yearsOfExperience: [null],
      address: [''],
    });
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.vetId = Number(id);
      this.vetService.getById(this.vetId).subscribe((v) => this.form.patchValue(v));
    }
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    const op = this.isEdit
      ? this.vetService.update(this.vetId!, this.form.value)
      : this.vetService.create(this.form.value);
    op.subscribe({
      next: () => { this.notify.success(this.isEdit ? 'Vet updated!' : 'Vet created!'); this.router.navigate(['/vets']); },
      error: () => (this.loading = false),
    });
  }
}
