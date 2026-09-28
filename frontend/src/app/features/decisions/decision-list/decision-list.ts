import {
  ChangeDetectorRef,
  Component,
  OnDestroy,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  Decision
} from '../../../core/models/decision.model';

import {
  DecisionService
} from '../../../core/services/decision.service';

import {
  Router
} from '@angular/router';

import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-decision-list',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe
  ],
  templateUrl: './decision-list.html',
  styleUrl: './decision-list.css'
})
export class DecisionListComponent
  implements OnInit, OnDestroy {

  private readonly decisionService =
    inject(DecisionService);

  private readonly changeDetector =
    inject(ChangeDetectorRef);

  private readonly router =
    inject(Router);

  private errorTimeout:
    ReturnType<typeof setTimeout> | null = null;

  private deleteErrorTimeout:
    ReturnType<typeof setTimeout> | null = null;

  decisions: Decision[] = [];

  isLoading = false;

  errorMessage = '';

  decisionToDelete: Decision | null = null;

  isDeleting = false;

  deleteErrorMessage = '';

  ngOnInit(): void {
    this.loadDecisions();
  }

  ngOnDestroy(): void {
    if (this.errorTimeout) {
      clearTimeout(this.errorTimeout);
      this.errorTimeout = null;
    }

    if (this.deleteErrorTimeout) {
      clearTimeout(this.deleteErrorTimeout);
      this.deleteErrorTimeout = null;
    }
  }

  private showError(message: string): void {
    if (this.errorTimeout) {
      clearTimeout(this.errorTimeout);
      this.errorTimeout = null;
    }

    this.errorMessage = message;

    if (!message) {
      this.changeDetector.markForCheck();
      return;
    }

    this.errorTimeout = setTimeout(() => {
      this.errorMessage = '';
      this.errorTimeout = null;

      this.changeDetector.markForCheck();
    }, 4000);
  }

  private showDeleteError(message: string): void {
    if (this.deleteErrorTimeout) {
      clearTimeout(this.deleteErrorTimeout);
      this.deleteErrorTimeout = null;
    }

    this.deleteErrorMessage = message;

    if (!message) {
      this.changeDetector.markForCheck();
      return;
    }

    this.deleteErrorTimeout = setTimeout(() => {
      this.deleteErrorMessage = '';
      this.deleteErrorTimeout = null;

      this.changeDetector.markForCheck();
    }, 4000);
  }

  loadDecisions(): void {
    this.isLoading = true;

    this.showError('');

    this.decisionService
      .getDecisions()
      .subscribe({

        next: (decisions) => {

          console.log(
            'Decisions received:',
            decisions
          );

          this.decisions = decisions;

          this.isLoading = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {

          console.error(
            'Failed to load decisions',
            error
          );

          this.showError(
            'Unable to load decisions. Please try again.'
          );

          this.isLoading = false;

          this.changeDetector.markForCheck();
        }

      });
  }

  createDecision(): void {
    this.router.navigate([
      '/decisions',
      'new',
      'wizard',
      'basic'
    ]);
  }

  openDecision(decisionId: number): void {
    this.router.navigate(
      [
        '/decisions',
        decisionId,
        'wizard',
        'preview'
      ],
      {
        queryParams: {
          fromDecisionList: 'true'
        }
      }
    );
  }

  editDecision(
    decision: Decision
  ): void {
    this.router.navigate([
      '/decisions',
      decision.id,
      'wizard',
      'basic'
    ]);
  }

  confirmDelete(
    decision: Decision
  ): void {
    this.decisionToDelete = decision;

    this.showDeleteError('');
  }

  cancelDelete(): void {
    if (this.isDeleting) {
      return;
    }

    this.decisionToDelete = null;

    this.showDeleteError('');
  }

  deleteDecision(): void {
    if (!this.decisionToDelete) {
      return;
    }

    const decisionId =
      this.decisionToDelete.id;

    this.isDeleting = true;

    this.showDeleteError('');

    this.decisionService
      .deleteDecision(decisionId)
      .subscribe({

        next: () => {

          this.decisions =
            this.decisions.filter(
              decision =>
                decision.id !== decisionId
            );

          this.decisionToDelete = null;

          this.isDeleting = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {

          console.error(
            'Failed to delete decision',
            error
          );

          this.showDeleteError(
            error?.error?.Error ??
            'Unable to delete the decision. Please try again.'
          );

          this.isDeleting = false;

          this.changeDetector.markForCheck();
        }

      });
  }
}