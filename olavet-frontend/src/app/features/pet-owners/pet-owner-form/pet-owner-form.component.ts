import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PetOwnerService } from '../../../core/services/pet-owner.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-pet-owner-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="form-page">
      <a routerLink="/pet-owners" class="back-link">← Back</a>
      <h1>{{ isEdit ? 'Edit Owner' : 'Add Owner' }}</h1>
      <form [formGroup]="form" (ngSubmit)="submit()" class="form-card">
        <div class="form-group"><label>Name *</label><input formControlName="ownerName" /></div>
        <div class="form-row">
          <div class="form-group"><label>Email *</label><input formControlName="email" type="email" /></div>
          <div class="form-group"><label>Phone *</label><input formControlName="contactNumber" /></div>
        </div>
        <div class="form-group"><label>Address</label><input formControlName="address" /></div>
        <div class="form-actions">
          <a routerLink="/pet-owners" class="btn btn-cancel">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading">{{ isEdit ? 'Update' : 'Create' }}</button>
        </div>
      </form>
    </div>
  `,
  styles: [`
    .form-page { max-width:640px; margin:0 auto; }
    .back-link { display:inline-block; margin-bottom:0.75rem; color:#667eea; text-decoration:none; }
    h1 { margin:0 0 1.5rem; font-size:1.5rem; }
    .form-card { background:#fff; border-radius:12px; padding:2rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); }
    .form-group { margin-bottom:1.25rem; display:flex; flex-direction:column;
      label { font-weight:500; margin-bottom:0.35rem; font-size:0.9rem; }
      input { padding:0.6rem 0.9rem; border:1px solid #e2e8f0; border-radius:8px; font-size:0.9rem; &:focus { outline:none; border-color:#667eea; } } }
    .form-row { display:grid; grid-template-columns:1fr 1fr; gap:1rem; }
    .form-actions { display:flex; justify-content:flex-end; gap:0.75rem; margin-top:1rem; }
    .btn { padding:0.55rem 1.25rem; border:none; border-radius:8px; font-weight:600; cursor:pointer; text-decoration:none;
      &-primary { background:#667eea; color:#fff; &:disabled { opacity:0.6; } }
      &-cancel { background:#edf2f7; color:#555; } }
  `],
})
export class PetOwnerFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private ownerService = inject(PetOwnerService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);
  form!: FormGroup;
  isEdit = false;
  ownerId: number | null = null;
  loading = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      ownerName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      contactNumber: ['', Validators.required],
      address: [''],
    });
    const id = this.route.snapshot.paramMap.get('id');
    if (id) { this.isEdit = true; this.ownerId = Number(id); this.ownerService.getById(this.ownerId).subscribe((o) => this.form.patchValue(o)); }
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    const op = this.isEdit ? this.ownerService.update(this.ownerId!, this.form.value) : this.ownerService.create(this.form.value);
    op.subscribe({
      next: () => { this.notify.success(this.isEdit ? 'Owner updated!' : 'Owner created!'); this.router.navigate(['/pet-owners']); },
      error: () => (this.loading = false),
    });
  }
}
