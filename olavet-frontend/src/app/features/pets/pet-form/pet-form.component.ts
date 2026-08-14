import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { PetService } from '../../../core/services/pet.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-pet-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './pet-form.component.html',
  styleUrl: './pet-form.component.scss',
})
export class PetFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private petService = inject(PetService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private notify = inject(NotificationService);

  form!: FormGroup;
  isEdit = false;
  petId: number | null = null;
  loading = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      species: ['', Validators.required],
      breed: ['', Validators.maxLength(100)],
      age: [null, [Validators.required, Validators.min(0)]],
      petWeight: [null, [Validators.min(0)]],
      color: [''],
      gender: ['', Validators.required],
      petOwnerId: [null, Validators.required],
    });

    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.isEdit = true;
      this.petId = Number(id);
      this.petService.getById(this.petId).subscribe((pet) => {
        this.form.patchValue(pet);
      });
    }
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.loading = true;
    const data = this.form.value;

    const op = this.isEdit
      ? this.petService.update(this.petId!, data)
      : this.petService.create(data);

    op.subscribe({
      next: () => {
        this.notify.success(this.isEdit ? 'Pet updated!' : 'Pet created!');
        this.router.navigate(['/pets']);
      },
      error: () => (this.loading = false),
    });
  }
}
